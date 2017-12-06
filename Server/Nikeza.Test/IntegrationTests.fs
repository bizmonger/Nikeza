module Nikeza.Server.Integration

open System
open System.IO
open FsUnit
open NUnit.Framework
open TestAPI
open DatabaseCommand
open Store
open Sql
open Read
open Model
open Literals
open Platforms
open StackOverflow.Suggestions
open Order
open Registration

[<SetUp>]
let setup() = registerProfile creatorRegistrationForm |> ignore

[<TearDown>]
let teardown() = cleanDataStore()

[<Test>]
let ``Sync stackoverflow`` () =
    
    // Setup
    let profileId =    registerProfile someForm
    let request =    { someSource with ProfileId= unbox profileId }
    let sourceId =     AddSource request |> execute
    let initialLinks = getLinks profileId
    let partialLinks = initialLinks |> List.take(List.length initialLinks - 1)
    
    // Test
    SyncSource request |> execute |> ignore

    // Verify
    profileId 
     |> getLinks 
     |> List.length 
     |> should be (greaterThan <| List.length initialLinks)

[<Test>]
let ``adding data source updates sync history`` () =
    
    // Setup
    let profileId = registerProfile someForm
    let source =  { someSource with AccessId= stackoverflowUserId
                                    ProfileId= profileId
                                    Platform=  StackOverflow |> PlatformToString }

    let sourceId = AddSource { source with ProfileId= unbox profileId } |> execute
    
    // Test
    getLastSynched <| Int32.Parse(sourceId)
    |> function
       | Some lastSynched -> lastSynched.ToShortDateString |> should equal DateTime.Now.Date.ToShortDateString
       | None -> Assert.Fail()

[<Test>]
let ``get last synch date from stackoverflow`` () =
    
    // Setup
    let profileId = registerProfile someForm
    let source =  { someSource with AccessId= stackoverflowUserId
                                    ProfileId= profileId
                                    Platform=  StackOverflow |> PlatformToString }

    let sourceId = AddSource { source with ProfileId= unbox profileId } |> execute
    
    // Test
    getLastSynched <| Int32.Parse(sourceId)
    |> function
       | Some lastSynched -> lastSynched.ToShortDateString |> should equal DateTime.Now.Date.ToShortDateString
       | None      -> Assert.Fail()

[<Test>]
let ``Removing data source updates portfolio`` () =
    
    // Setup
    let profileId = registerProfile someForm
    let sourceId =  AddSource { someSource with ProfileId= unbox profileId } |> execute
    
    // Test
    RemoveSource { Id = Int32.Parse(sourceId) } |> execute |> ignore

    // Verify
    profileId |> getLinks |> List.isEmpty |> should equal true

[<Test>]
let ``New members have default subscription`` () =

    register someForm |> function
    | Success profile ->
        let subscription = profile.Id |> getSubscriptions |> List.head
        let creator =      getProfileByEmail creatorEmail

        subscription.Profile.Id |> should equal creator.Value.Id
    | _ -> Assert.Fail()

[<Test>]
let ``get links from datasource`` () =

    // Setup
    let profileId = registerProfile someForm
    let source =  { someSource with AccessId=  wordpressUserId
                                    ProfileId= profileId
                                    Platform=  WordPress |> PlatformToString }
    // Test
    AddSource { source with ProfileId= unbox profileId } |> execute |> ignore
    let links = source.ProfileId |> Store.linksFrom source.Platform

    // Verify
    links |> List.isEmpty |> should equal false

[<Test>]
let ``Adding data source maintains topics`` () =

    // Setup
    let profileId = registerProfile someForm
    let source =  { someSource with AccessId=  wordpressUserId
                                    ProfileId= profileId
                                    Platform=  WordPress |> PlatformToString }
    // Test
    AddSource { source with ProfileId= unbox profileId } |> execute |> ignore
    let links = source.ProfileId |> Store.linksFrom source.Platform |> List.filter(fun l -> l.Title = "F#: Revisiting the Vending Machine Kata")

    // Verify
    let link = links.[0]
    link.Topics |> List.isEmpty |> should equal false


[<Test>]
let ``get topics from text`` () =

    suggestionsFromText "F#: Revisiting the Vending Machine Kata." 
     |> List.contains "f#" 
     |> should equal true


[<Test>]
let ``Load links from RSS feed`` () =

    // Setup
    let profileId = registerProfile someForm
    let source =  { someSource with AccessId=  rssFeedId
                                    ProfileId= profileId
                                    Platform=  RSSFeed |> PlatformToString }
    // Test
    AddSource { source with ProfileId= unbox profileId } |> execute |> ignore
    let links = source.ProfileId |> Store.linksFrom source.Platform |> List.toSeq

    // Verify
    links |> Seq.length |> should (be greaterThan) 0

[<Test>]
let ``Get profile image from StackOverflow`` () =

    StackOverflow 
     |> getThumbnail stackoverflowUserId
     |> should equal someStackoverflowImage

[<Test>]
let ``Get profile image from YouTube`` () =

    YouTube |> getThumbnail youtubeUserId
            |> should equal someYoutubeImage

[<Test>]
let ``Get profile image from WordPress`` () =

    WordPress |> getThumbnail wordpressUserId
              |> should equal someWordpressImage

[<Test>]
let ``Get profile image from Medium`` () =

    Medium |> getThumbnail mediumUserId
           |> should equal someMediumImage

[<Test>]
let ``Load links from Medium`` () =

    // Setup
    let profileId = registerProfile someForm
    let source =  { someSource with AccessId=  mediumUserId
                                    ProfileId= profileId
                                    Platform=  Medium |> PlatformToString }
    // Test
    AddSource { source with ProfileId= unbox profileId } |> execute |> ignore
    let links = source.ProfileId |> Store.linksFrom source.Platform |> List.toSeq

    // Verify
    links |> Seq.length |> should (be greaterThan) 0

[<Test>]
let ``Load links from WordPress`` () =

    // Setup
    let profileId = registerProfile someForm
    let source =  { someSource with AccessId=  wordpressUserId
                                    ProfileId= profileId
                                    Platform=  WordPress |> PlatformToString }
    // Test
    AddSource { source with ProfileId= unbox profileId } |> execute |> ignore
    let links = source.ProfileId |> Store.linksFrom source.Platform |> List.toSeq

    // Verify
    links |> Seq.length |> should (be greaterThan) 0

[<Test>]
let ``Load links from StackOverflow`` () =

    // Setup
    let profileId = registerProfile someForm
    let source =  { someSource with AccessId= stackoverflowUserId
                                    ProfileId= profileId
                                    Platform=  StackOverflow |> PlatformToString }
    // Test
    AddSource { source with ProfileId= unbox profileId } |> execute |> ignore
    let links = source.ProfileId |> Store.linksFrom source.Platform |> List.toSeq

    // Verify
    links |> Seq.length |> should (be greaterThan) 0

[<Test>]
let ``Load links from YouTube`` () =

    // Setup
    let profileId = registerProfile someForm
    let source =  { someSource with AccessId=  File.ReadAllText(ChannelIdFile)
                                    ProfileId= profileId
                                    Platform=  YouTube |> PlatformToString }
    // Test
    AddSource { source with ProfileId= unbox profileId } |> execute |> ignore
    let links = source.ProfileId |> Store.linksFrom source.Platform |> List.toSeq

    // Verify
    links |> Seq.isEmpty |> should equal false

[<Test>]
let ``Read YouTube APIKey file`` () =
    let text = File.ReadAllText(KeyFile_YouTube)
    text.Length |> should (be greaterThan) 0

[<Test>]
let ``Read YouTube AccessId file`` () =
    let text = File.ReadAllText(ChannelIdFile)
    text.Length |> should (be greaterThan) 0

[<Test>]
let ``Subscriber observes recent provider link`` () =

    // Setup
    let profileId =    registerProfile someForm
    let subscriberId = registerProfile someSubscriberRegistrationForm

    Follow { FollowRequest.ProfileId= profileId
             FollowRequest.SubscriberId= subscriberId 
           } |> execute |> ignore

    let linkId = AddLink  { someLink with ProfileId = profileId } |> execute

    // Test
    let link = { SubscriberId= subscriberId; LinkIds= [Int32.Parse(linkId)] }
    let linkObservedIds = ObserveLinks link |> execute

    // Verify
    linkObservedIds |> should equal linkId

[<Test>]
let ``No recent links after subscriber observes new link`` () =

    // Setup
    let profileId =    registerProfile someForm
    let subscriberId = registerProfile someSubscriberRegistrationForm

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
    let profileId =    registerProfile someForm
    let subscriberId = registerProfile someSubscriberRegistrationForm

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
    let profileId =    registerProfile someForm
    let subscriberId = registerProfile someSubscriberRegistrationForm

    execute ( Follow { FollowRequest.ProfileId= profileId; FollowRequest.SubscriberId= subscriberId }) |> ignore

    // Test
    execute ( Unsubscribe { UnsubscribeRequest.SubscriberId= subscriberId; UnsubscribeRequest.ProfileId= profileId }) |> ignore

    // Verify
    let sql = @"SELECT SubscriberId, ProfileId
                FROM   Subscription
                WHERE  SubscriberId = @SubscriberId
                AND    ProfileId =    @ProfileId"

    let (connection,command) = createCommand sql connectionString

    try
        connection.Open()
        command.Parameters.AddWithValue("@SubscriberId", subscriberId) |> ignore
        command.Parameters.AddWithValue("@ProfileId",    profileId)   |> ignore

        use reader = command.ExecuteReader()
        reader.Read() |> should equal false

    // Teardown
    finally dispose connection command

[<Test>]
let ``Add featured link`` () =

    //Setup
    registerProfile someForm |> ignore

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
    let profileId = registerProfile someForm

    // Test
    AddLink { someLink with Topics= [someProviderTopic]; ProfileId= profileId } |> execute |> ignore

    // Verify
    match getTopic someTopic.Name with
    | Some topic -> topic.Name.ToLower() |> should equal (someTopic.Name.ToLower())
    | None       -> Assert.Fail()

[<Test>]
let ``Remove link`` () =

    //Setup
    registerProfile someForm |> ignore
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
    registerProfile someForm |> ignore

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
    let data = { someForm with FirstName="Scott"; LastName="Nimrod" }

    // Test
    registerProfile data |> ignore

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
    let data = { someForm with FirstName="Scott"; LastName="Nimrod" }
    let profileId = registerProfile data

    let profile = (getProfile profileId).Value
    
    // Test
    UpdateProfile { Id =  unbox profileId
                    FirstName =  profile.FirstName
                    LastName =   modifiedName
                    Bio =        profile.Bio
                    Email =      profile.Email
                    ImageUrl=    profile.ImageUrl
                    Sources =    profile.Sources } |> execute |> ignore
    // Verify
    let sql = @"SELECT LastName FROM [dbo].[Profile] WHERE  Id = @Id"
    let (readConnection,readCommand) = createCommand sql connectionString
    try readConnection.Open()
        readCommand.Parameters.AddWithValue("@Id", profileId) |> ignore
        
        use reader = readCommand |> prepareReader
        reader.GetString(0) = modifiedName |> should equal true

    // Teardown
    finally dispose readConnection readCommand

[<Test>]
let ``Get links of provider`` () =

    //Setup
    let profileId = registerProfile someForm
    AddLink { someLink with ProfileId= unbox profileId } |> execute |> ignore
    
    // Test
    let links = profileId |> getLinks

    // Verify
    let linkFound = links |> Seq.head
    linkFound.ProfileId  |> should equal profileId

[<Test>]
let ``Get followers`` () =

    // Setup
    let profileId =    registerProfile someForm
    let subscriberId = registerProfile someSubscriberRegistrationForm
    
    Follow { FollowRequest.ProfileId=   profileId
             FollowRequest.SubscriberId= subscriberId } |> execute |> ignore

    // Test
    let follower = profileId |> getFollowers |> List.head
    
    // Verify
    follower.Profile.Id |> should equal subscriberId

[<Test>]
let ``Get subscriptions`` () =

    // Setup
    let profileId =    registerProfile someForm
    let subscriberId = registerProfile someSubscriberRegistrationForm

    Follow { FollowRequest.ProfileId=   profileId
             FollowRequest.SubscriberId= subscriberId } |> execute |> ignore

    // Test
    let subscription = subscriberId |> getSubscriptions |> List.head
    
    // Verify
    subscription.Profile.Id |> should equal profileId

[<Test>]
let ``Get profiles`` () =

    // Setup
    registerProfile { someForm with FirstName= "profile1" } |> ignore
    registerProfile { someForm with FirstName= "profile2" } |> ignore

    // Test
    let profiles = getAllProfiles()
    
    // Verify
    profiles |> List.length |> should equal 2

[<Test>]
let ``Get profile`` () =

    registerProfile someForm
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
    let profileId = registerProfile someForm
    let source =  { someSource with AccessId= File.ReadAllText(ChannelIdFile) }
    AddSource { source with ProfileId= unbox profileId } |> execute |> ignore

    // Test
    let links = Store.linksFrom source.Platform profileId

    // Verify
    links |> List.isEmpty |> should equal false

[<Test>]
let ``Add data source`` () =

    // Setup
    let profileId = registerProfile someForm
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
let ``Adding data source results in links with topics found`` () =

    //Setup
    let profileId = registerProfile someForm

    // Test
    let sourceId = AddSource { someSource with ProfileId= unbox profileId } |> execute

    // Verify
    getSource sourceId |> function
    | Some source -> source.Links |> Seq.forall(fun l -> l.Topics |> List.isEmpty |> not) |> should equal false
    | None        -> Assert.Fail()
    
[<Test>]
let ``Get sources`` () =

    //Setup
    let profileId = registerProfile someForm
    AddSource { someSource with ProfileId = unbox profileId } |> execute |> ignore

    // Test
    let sources = profileId |> getSources
    
    // Verify
    sources |> List.isEmpty |> should equal false

[<Test>]
let ``Remove source`` () =

    //Setup
    let profileId = registerProfile someForm
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


[<Test>]
let ``Add featured topic`` () =

    //Setup
    let profileId = registerProfile someForm

    let link = { someLink with Topics= [someProviderTopic]; ProfileId= profileId }
    AddLink link |> execute |> ignore

    let topic =     link.Topics.Head.Name
    let request = { ProfileId=profileId; Names=[topic] }

    // Test
    let featuredTopicId = UpdateTopics request |> execute

    // Verify
    Int32.Parse(featuredTopicId) |> should (be greaterThan) 0

[<Test>]
let ``Remove featured topic`` () =

    //Setup
    let profileId = registerProfile someForm

    let link = { someLink with Topics= [someProviderTopic]; ProfileId= profileId }
    AddLink link |> execute |> ignore

    let topic =     link.Topics.Head.Name
    let request = { ProfileId=profileId; Names= [] }
    UpdateTopics request |> execute |> ignore

    // Test
    UpdateTopics { ProfileId=profileId; Names= [] } |> execute |> ignore

    // Verify
    let featuredTopics = getFeaturedTopics profileId
    featuredTopics |> List.isEmpty |> should equal true
    

[<Test>]
let ``Fetching provider includes their featured topics`` () =

    //Setup
    let profileId = registerProfile someForm
    let link =    { someLink with Topics= [{someProviderTopic with IsFeatured= true}]; ProfileId=profileId |> string }

    // Test
    AddLink link |> execute |> ignore

    // Verify
    let provider = getProviders().[0]
    provider.Topics |> List.isEmpty |> should equal false


[<Test>]
let ``Logging into portal retrieves portfolio`` () =

    //Setup
    let profileId = registerProfile someForm
    let link =    { someLink with Topics= [someProviderTopic]; ProfileId= profileId }
    AddLink link |> execute |> ignore

    // Test
    match login someForm.Email with
          | Some provider ->
             if  provider.Portfolio |> isEmpty
                then Assert.Fail()
                else ()
          | None -> Assert.Fail()

[<EntryPoint>]
let main argv =
    cleanDataStore()                      
    ``Add data source`` ()
    0