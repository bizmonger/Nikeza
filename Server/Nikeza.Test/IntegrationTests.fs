module Integration

open System
open System.IO
open FsUnit
open NUnit.Framework
open Nikeza.TestAPI
open Nikeza.Server.Command
open Nikeza.Server.Store
open Nikeza.Server.Sql
open Nikeza.Server.Read
open Nikeza.Server.Model
open Nikeza.Server.Literals

[<TearDownAttribute>]
let teardown() = cleanDataStore()

[<Test>]
let ``Parse Medium JSON`` () =

    let parseValue (line:string) =
        if line.Contains(":")
            then Some <| line.Split(':')
                             .[1]
                             .Replace("\"", "")
                             .Trim()
                             .TrimEnd(',')
            else None
                     
    let getPostBlock (text:string) =
        let postsIndex =  text.IndexOf("\"Post\": {") + 11
        let partial =     text.Substring(postsIndex, text.Length - postsIndex)
        let postEndIndex= partial.IndexOf("},")
        let postBlock=    partial.Substring(0, postEndIndex)
        postBlock

    let createLink (postBlock:string) =

        let getTagsBlock (postBlock:string) =
            let startIndex = postBlock.IndexOf("\"tags\": [")
            let block = postBlock.Substring(startIndex, postBlock.Length - startIndex)
            let endIndex = block.IndexOf("],")
            let tagsBlock = block.Substring(0, endIndex)
            tagsBlock

        let getTagBlock (tagsBlock:string) =
            let startIndex = tagsBlock.IndexOf('{')

            if startIndex >= 0
                then let endIndex =   tagsBlock.IndexOf('}')
                     let tagBlock =   tagsBlock.Substring(startIndex, endIndex)
                     Some tagBlock
                else None

        let rec getTags (postBlock:string) (tags: (string option) list) =

            let rec getTag (postBlock:string) : string option =
                let tagBlock =   postBlock |> getTagsBlock |> getTagBlock

                match tagBlock with
                    | None       -> None
                    | Some block ->
                        if block.Contains("\"slug\":")
                            then let tagParts = block.Split('\n')
                                 let tag =      parseValue(tagParts.[2])
                                 tag
                            else let truncated = postBlock.Replace(block, "")
                                 getTag truncated
            
            let tag =      getTag postBlock
            let tagBlock = postBlock |> getTagsBlock |> getTagBlock

            match tagBlock with
            | None       -> tags
            | Some block ->
                if block.Contains("\"slug\":")
                    then let truncated = postBlock.Replace(block, "")
                         tag::tags |> getTags truncated
                    else let nextBlock = postBlock |> getTagsBlock |> getTagBlock
                         match nextBlock with
                         | Some t -> 
                            let truncated = postBlock.Replace(t, "")
                            tag::tags |> getTags truncated
                         | None   -> tag::tags

        let postParts = postBlock.Split("\n")

        let topics = [] |> getTags postBlock 
                        |> List.choose(fun tag -> match tag with
                                                  | Some t -> Some { Id= -1; Name=t; }
                                                  | None   -> None
                                      )
        { Id= -1
          ProfileId=   "to be derived..."
          Title=        parseValue(postParts.[5]) |> function | Some title -> title | None -> ""
          Description=  ""
          Url=          parseValue(postParts.[1]) |> function | Some title -> "{0}" + title | None -> ""
          Topics=       topics |>List.toSeq |> Seq.distinct |> Seq.toList
          ContentType= "Article"
          IsFeatured=   false
        }

    let remainingText nextTagIndex (text:string) =
        let newIndex =       if nextTagIndex >= 300 then nextTagIndex - 300 else nextTagIndex
        let nextPostTemp =   text.Substring(newIndex, text.Length - newIndex)
        let newEndIndex =    nextPostTemp.IndexOf(": {")
        let nextPost =       nextPostTemp.Substring(newEndIndex, nextPostTemp.Length - newEndIndex)
        nextPost

    let getNextPost (text:string) (postBlock:string) =
        let tagsIndex =      text.IndexOf("\"tags\": [")
        let tagsBlock1 =     text.Substring(tagsIndex, text.Length - tagsIndex)
        let tagsEndIndex=    tagsBlock1.IndexOf("],")
        let removeTagBlock=  tagsBlock1.Substring(0, tagsEndIndex)
        let truncatedText1 = text.Replace(removeTagBlock, "")
        let truncatedText2 = truncatedText1.Replace(postBlock, "")

        let nextPostIndex = truncatedText2.IndexOf("\"homeCollectionId\":")
        let nextPost = remainingText nextPostIndex truncatedText2
        nextPost

    let rec linksFrom (partial:string) (originalContent:string) links =

        let identifier =   "\"homeCollectionId\":"
        let nextTagIndex =  partial.IndexOf(identifier)

        if nextTagIndex >= 0
            then let nextPost = remainingText nextTagIndex partial

                 if nextPost.Contains(identifier)
                    then let endIndex =        partial.IndexOf("\"homeCollectionId\":", partial.IndexOf("\"homeCollectionId\":") + 1)
                         let entirePostBlock = partial.Substring(0, endIndex - 300)
                         let link =            createLink entirePostBlock
                         let postBlock =       getPostBlock originalContent
                         let content =         getNextPost nextPost postBlock
                         
                         [link] |> List.append links 
                                |> linksFrom content originalContent

                    else let link = createLink partial
                         List.append links [link]
            else links

    let getLinks url =
        let text =        File.ReadAllText(@"C:\Nikeza\Medium_json_examle.txt")
        let postsIndex =  text.IndexOf("\"Post\": {") + 11
        let postsBlock =  text.Substring(postsIndex, text.Length - postsIndex)
        let links =       [] |> linksFrom postsBlock text
        links

    let links = getLinks @"C:\Nikeza\Medium_json_examle.txt"
        
    links |> List.length |> should (be greaterThan) 0

[<Test>]
let ``Read YouTube APIKey file`` () =
    let text = File.ReadAllText(KeyFile_YouTube)
    text.Length |> should (be greaterThan) 0

[<Test>]
let ``Read YouTube AccessId file`` () =
    let text = File.ReadAllText(ChannelIdFile)
    text.Length |> should (be greaterThan) 0

[<Test>]
let ``Subscriber observes provider link`` () =

    // Setup
    let profileId =    Register someProfile    |> execute
    let subscriberId = Register someSubscriber |> execute

    Follow { FollowRequest.ProfileId= profileId
             FollowRequest.SubscriberId= subscriberId 
           } |> execute |> ignore

    let linkId = AddLink  { someLink with ProfileId = profileId } |> execute

    // Test
    let link = { SubscriberId= subscriberId; LinkIds=[Int32.Parse(linkId)] }
    let linkObservedIds = ObserveLinks link |> execute

    // Verify
    linkObservedIds |> should equal linkId

[<Test>]
let ``No recent links after subscriber observes new link`` () =

    // Setup
    let profileId =    Register someProfile    |> execute
    let subscriberId = Register someSubscriber |> execute

    Follow { FollowRequest.ProfileId= profileId
             FollowRequest.SubscriberId= subscriberId 
           } |> execute |> ignore

    let linkId = AddLink  { someLink with ProfileId = profileId } |> execute
    let link = { SubscriberId= subscriberId; LinkIds=[Int32.Parse(linkId)] }

    ObserveLinks link |> execute |> ignore

    // Test
    let recentLinks = subscriberId |> getRecent

    // Verify
    recentLinks |> List.isEmpty |> should equal true

[<Test>]
let ``Follow Provider`` () =

    // Setup
    let profileId =    Register someProfile    |> execute
    let subscriberId = Register someSubscriber |> execute

    // Test
    Follow { FollowRequest.ProfileId= profileId
             FollowRequest.SubscriberId= subscriberId 
           } |> execute |> ignore

    // Verify
    let sql = @"SELECT SubscriberId, ProfileId
                FROM   Subscription
                WHERE  SubscriberId = @SubscriberId
                AND    ProfileId =    @ProfileId"

    let (connection,command) = createCommand sql connectionString

    try
        connection.Open()
        command.Parameters.AddWithValue("@SubscriberId", subscriberId) |> ignore
        command.Parameters.AddWithValue("@ProfileId",   profileId)   |> ignore

        use reader = command |> prepareReader
        let entryAdded = reader.GetInt32(0) = Int32.Parse (subscriberId) && 
                         reader.GetInt32(1) = Int32.Parse (profileId)

        entryAdded |> should equal true

    // Teardown
    finally dispose connection command


[<Test>]
let ``Unsubscribe from Provider`` () =

    // Setup
    let profileId =    Register someProfile    |> execute 
    let subscriberId = Register someSubscriber |> execute

    execute ( Follow { FollowRequest.ProfileId= profileId; FollowRequest.SubscriberId= subscriberId }) |> ignore

    // Test
    execute ( Unsubscribe { UnsubscribeRequest.SubscriberId= subscriberId; UnsubscribeRequest.ProfileId= profileId }) |> ignore

    // Verify
    let sql = @"SELECT SubscriberId, ProfileId
                FROM   Subscription
                WHERE  SubscriberId = @SubscriberId
                AND    ProfileId =   @ProfileId"

    let (connection,command) = createCommand sql connectionString

    try
        connection.Open()
        command.Parameters.AddWithValue("@SubscriberId", subscriberId) |> ignore
        command.Parameters.AddWithValue("@ProfileId",   profileId)   |> ignore

        use reader = command.ExecuteReader()
        reader.Read() |> should equal false

    // Teardown
    finally dispose connection command

[<Test>]
let ``Add featured link`` () =

    //Setup
    Register someProfile |> execute |> ignore

    let lastId = AddLink  someLink |> execute
    let data = { LinkId=Int32.Parse(lastId); IsFeatured=true }

    // Test
    FeatureLink data |> execute |> ignore

    // Verify
    let sql = @"SELECT Id, IsFeatured
                FROM   Link
                WHERE  Id  = @id
                AND    IsFeatured = @isFeatured"

    let (connection,command) = createCommand sql connectionString

    try
        connection.Open()
        command.Parameters.AddWithValue("@id",         data.LinkId)     |> ignore
        command.Parameters.AddWithValue("@isFeatured", data.IsFeatured) |> ignore

        use reader = command |> prepareReader
        let isFeatured = reader.GetBoolean(1)
        isFeatured |> should equal true

    // Teardown
    finally dispose connection command

[<Test>]
let ``Adding link results in new topics added to database`` () =

    //Setup
    Register someProfile |> execute |> ignore

    // Test
    AddLink { someLink with Topics= [someTopic] } |> execute |> ignore

    // Verify
    match getTopic someTopic.Name with
    | Some topic -> topic.Name |> should equal someTopic.Name
    | None       -> Assert.Fail()

[<Test>]
let ``Remove link`` () =

    //Setup
    Register someProfile |> execute |> ignore
    let linkId = AddLink someLink |> execute
    
    // Test
    RemoveLink { LinkId = Int32.Parse(linkId) } |> execute |> ignore

    // Verify
    let sql = @"SELECT Id, IsFeatured
                FROM   Link
                WHERE  Id = @id"

    let (connection,command) = createCommand sql connectionString

    try
        connection.Open()
        command.Parameters.AddWithValue("@id", linkId) |> ignore

        use reader = command.ExecuteReader()
        reader.Read() |> should equal false

    // Teardown
    finally dispose connection command

[<Test>]
let ``Unfeature Link`` () =
    
    //Setup
    Register someProfile |> execute |> ignore

    let linkId = AddLink  someLink |> execute
    let data = { LinkId=Int32.Parse(linkId); IsFeatured=false }

    // Test
    FeatureLink data |> execute |> ignore

    // Verify
    let sql = @"SELECT Id, IsFeatured
                FROM   Link
                WHERE  Id  = @id
                AND    IsFeatured = @isFeatured"

    let (connection,command) = createCommand sql connectionString
    try
        connection.Open()
        command.Parameters.AddWithValue("@id",         data.LinkId)     |> ignore
        command.Parameters.AddWithValue("@isFeatured", data.IsFeatured) |> ignore

        use reader = command |> prepareReader
        let isFeatured = reader.GetBoolean(1)
        isFeatured |> should equal false

    // Teardown
    finally dispose connection command

[<Test>]
let ``Register Profile`` () =

    // Setup
    let data = { someProfile with FirstName="Scott"; LastName="Nimrod" }

    // Test
    Register data |> execute |> ignore

    // Verify
    let sql = @"SELECT FirstName, LastName
                FROM   Profile
                WHERE  FirstName = @FirstName
                AND    LastName  = @LastName"

    let (connection,command) = createCommand sql connectionString
    try
        connection.Open()
        command.Parameters.AddWithValue("@FirstName", data.FirstName) |> ignore
        command.Parameters.AddWithValue("@LastName",  data.LastName)  |> ignore
        
        use reader = command |> prepareReader
        let isRegistered = (data.FirstName, data.LastName) = (reader.GetString(0), reader.GetString(1))
        isRegistered |> should equal true

    // Teardown
    finally dispose connection command

[<Test>]
let ``Update profile`` () =
    
    // Setup
    let modifiedName = "MODIFIED_NAME"
    let data = { someProfile with FirstName="Scott"; LastName="Nimrod" }
    let lastId = Register data |> execute
    // Test
    UpdateProfile { ProfileId =  unbox lastId
                    FirstName =  data.FirstName
                    LastName =   modifiedName
                    Bio =        data.Bio
                    Email =      data.Email
                    ImageUrl=    data.ImageUrl
                    Sources =    data.Sources } |> execute |> ignore
    // Verify
    let sql = @"SELECT LastName FROM [dbo].[Profile] WHERE  Id = @Id"
    let (readConnection,readCommand) = createCommand sql connectionString
    try readConnection.Open()
        readCommand.Parameters.AddWithValue("@Id", lastId) |> ignore
        
        use reader = readCommand |> prepareReader
        reader.GetString(0) = modifiedName |> should equal true

    // Teardown
    finally dispose readConnection readCommand

[<Test>]
let ``Get links of provider`` () =

    //Setup
    let profileId = Register someProfile |> execute
    AddLink  { someLink with ProfileId= unbox profileId } |> execute |> ignore
    
    // Test
    let links = profileId |> getLinks

    // Verify
    let linkFound = links |> Seq.head
    linkFound.ProfileId  |> should equal profileId

[<Test>]
let ``Get followers`` () =

    // Setup
    let profileId =    Register someProfile    |> execute
    let subscriberId = Register someSubscriber |> execute
    
    Follow { FollowRequest.ProfileId=   profileId
             FollowRequest.SubscriberId= subscriberId } |> execute |> ignore

    // Test
    let follower = profileId |> getFollowers |> List.head
    
    // Verify
    follower.ProfileId |> should equal subscriberId

[<Test>]
let ``Get subscriptions`` () =

    // Setup
    let profileId =   Register someProfile   |> execute
    let subscriberId = Register someSubscriber |> execute

    Follow { FollowRequest.ProfileId=   profileId
             FollowRequest.SubscriberId= subscriberId } |> execute |> ignore

    // Test
    let subscription = subscriberId |> getSubscriptions |> List.head
    
    // Verify
    subscription.ProfileId |> should equal profileId

[<Test>]
let ``Get profiles`` () =

    // Setup
    Register { someProfile with FirstName= "profile1" } |> execute |> ignore
    Register { someProfile with FirstName= "profile2" } |> execute |> ignore

    // Test
    let profiles = getAllProfiles()
    
    // Verify
    profiles |> List.length |> should equal 2

[<Test>]
let ``Get profile`` () =

    Register someProfile 
    |> execute
    |> getProfile
    |> function | Some _ -> ()
                | None   -> Assert.Fail()

[<Test>]
let ``Get platforms`` () =

    getPlatforms() 
    |> List.isEmpty 
    |> should equal false

[<Test>]
let ``Adding data source results in links saved`` () =

    // Setup
    let profileId = Register someProfile |> execute
    let source =  { someSource with AccessId= File.ReadAllText(ChannelIdFile) }
    AddSource { source with ProfileId= unbox profileId } |> execute |> ignore

    // Test
    let links = linksFrom source.Platform profileId

    // Verify
    links |> List.isEmpty |> should equal false

[<Test>]
let ``Add data source`` () =

    // Setup
    let profileId = Register someProfile |> execute
    let source =  { someSource with AccessId= File.ReadAllText(ChannelIdFile) }

    // Test
    let sourceId = AddSource { source with ProfileId= unbox profileId } |> execute

    // Verify
    let sql = @"SELECT Id FROM [dbo].[Source] WHERE Id = @id"
    let (connection,command) = createCommand sql connectionString

    try connection.Open()
        command.Parameters.AddWithValue("@id", sourceId) |> ignore

        use reader = command.ExecuteReader()
        reader.Read() |> should equal true

    // Teardown
    finally dispose connection command

[<Test>]
let ``Adding data source results in links found`` () =

    //Setup
    let profileId = Register someProfile |> execute

    // Test
    let sourceId = AddSource { someSource with ProfileId= unbox profileId } |> execute

    // Verify
    getSource sourceId |> function
    | Some source -> source.Links |> Seq.isEmpty |> should equal false
    | None        -> Assert.Fail()
    
[<Test>]
let ``Get sources`` () =

    //Setup
    let profileId = execute <| Register someProfile
    AddSource { someSource with ProfileId = unbox profileId } |> execute |> ignore

    // Test
    let sources = profileId |> getSources
    
    // Verify
    sources |> List.isEmpty |> should equal false

[<Test>]
let ``Remove source`` () =

    //Setup
    let profileId = execute <| Register someProfile
    let sourceId =  AddSource { someSource with ProfileId= unbox profileId } |> execute
    
    // Test
    RemoveSource { Id = Int32.Parse(sourceId) } |> execute |> ignore

    // Verify
    let sql = @"SELECT Id FROM [dbo].[Source] WHERE  Id  = @id"
    let (connection,command) = createCommand sql connectionString

    try connection.Open()
        command.Parameters.AddWithValue("@id", sourceId) |> ignore

        use reader = command.ExecuteReader()
        reader.Read() |> should equal false

    // Teardown
    finally dispose connection command

[<EntryPoint>]
let main argv =
    cleanDataStore()                      
    ``Add data source`` ()
    0