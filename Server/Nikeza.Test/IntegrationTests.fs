module Integration

open System
open FsUnit
open NUnit.Framework
open Nikeza.Server.Sql
open Nikeza.Server.DataRead
open Nikeza.Server.Models
open Nikeza.Server.DataStore
open System.Data.SqlClient
open Nikeza.TestAPI

module DataStore = Nikeza.Server.DataStore

// https://github.com/nunit/dotnet-test-nunit

[<TearDownAttribute>]
let teardown() = 
    cleanDataStore()

[<Test>]
let ``Follow Provider`` () =

    // Setup
    execute <| Register someProvider
    let providerId =   getLastId "Profile"
    
    execute <| Register someSubscriber
    let subscriberId = getLastId "Profile"

    // Test
    execute <| Follow { FollowRequest.ProviderId= providerId; FollowRequest.SubscriberId= subscriberId }

    // Verify
    let sql = @"SELECT SubscriberId, ProviderId
                FROM   Subscription
                WHERE  SubscriberId = @SubscriberId
                AND    ProviderId =   @ProviderId"

    let (connection,command) = createCommand sql connectionString

    try
        connection.Open()
        command.Parameters.AddWithValue("@SubscriberId", subscriberId) |> ignore
        command.Parameters.AddWithValue("@ProviderId",   providerId)   |> ignore

        use reader = command |> prepareReader
        let entryAdded = reader.GetInt32(0) = subscriberId && 
                         reader.GetInt32(1) = providerId

        entryAdded |> should equal true

    // Teardown
    finally dispose connection command


[<Test>]
let ``Unsubscribe from Provider`` () =

    // Setup
    execute <| Register someProvider
    let providerId =   getLastId "Profile"
    
    execute <| Register someSubscriber
    let subscriberId = getLastId "Profile"

    execute <| Follow { FollowRequest.ProviderId= providerId; FollowRequest.SubscriberId= subscriberId }

    // Test
    execute <| Unsubscribe { UnsubscribeRequest.SubscriberId= subscriberId; UnsubscribeRequest.ProviderId= providerId }

    // Verify
    let sql = @"SELECT SubscriberId, ProviderId
                FROM   Subscription
                WHERE  SubscriberId = @SubscriberId
                AND    ProviderId =   @ProviderId"

    let (connection,command) = createCommand sql connectionString

    try
        connection.Open()
        command.Parameters.AddWithValue("@SubscriberId", subscriberId) |> ignore
        command.Parameters.AddWithValue("@ProviderId",   providerId)   |> ignore

        use reader = command.ExecuteReader()
        reader.Read() |> should equal false

    // Teardown
    finally dispose connection command

[<Test>]
let ``Add featured link`` () =

    //Setup
    execute <| Register someProvider
    execute <| AddLink  someLink

    let lastId =  getLastId "Link"
    let data =  { LinkId=lastId; IsFeatured=true }

    // Test
    execute <| FeatureLink data

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
let ``Remove link`` () =

    //Setup
    execute <| Register someProvider
    execute <| AddLink  someLink
    
    let linkId =  getLastId "Link"

    // Test
    execute <| RemoveLink  {LinkId = linkId}

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
    execute <| Register someProvider
    execute <| AddLink  someLink

    let data = { LinkId=getLastId "Link"; IsFeatured=false }

    // Test
    execute <| FeatureLink data

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
    let data = { someProvider with FirstName="Scott"; LastName="Nimrod" }

    // Test
    execute <| Register data

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
    let data = { someProvider with FirstName="Scott"; LastName="Nimrod" }
    execute <| Register data

    let lastId =  getLastId "Profile"

    // Test
    execute <| UpdateProfile { ProfileId =  lastId
                               FirstName =  data.FirstName
                               LastName =   modifiedName
                               Bio =        data.Bio
                               Email =      data.Email
                               ImageUrl=    data.ImageUrl
                             }
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
    execute <| Register someProvider

    let providerId = getLastId "Profile"

    execute <| AddLink  { someLink with ProviderId= providerId }
    
    // Test
    let links = providerId |> getLinks

    // Verify
    let linkFound = links |> Seq.head
    linkFound.ProviderId  |> should equal providerId

[<Test>]
let ``Get followers`` () =

    // Setup
    execute <| Register someProvider
    let providerId =   getLastId "Profile"
    
    execute <| Register someSubscriber
    let subscriberId = getLastId "Profile"

    execute <| Follow { FollowRequest.ProviderId=   providerId; 
                        FollowRequest.SubscriberId= subscriberId }

    // Test
    let follower = providerId |> getFollowers |> List.head
    
    // Verify
    follower.ProfileId |> should equal subscriberId

[<Test>]
let ``Get subscriptions`` () =

    // Setup
    execute <| Register someProvider
    let providerId =   getLastId "Profile"
    
    execute <| Register someSubscriber
    let subscriberId = getLastId "Profile"

    execute <| Follow { FollowRequest.ProviderId=   providerId; 
                        FollowRequest.SubscriberId= subscriberId }

    // Test
    let subscription = subscriberId |> getSubscriptions |> List.head
    
    // Verify
    subscription.ProfileId |> should equal providerId

[<Test>]
let ``Get providers`` () =

    // Setup
    execute <| Register { someProvider with FirstName= "Provider1" }
    execute <| Register { someProvider with FirstName= "Provider2" }

    // Test
    let providers = getProviders()
    
    // Verify
    providers |> List.length |> should equal 2

[<Test>]
let ``Get provider`` () =

    // Setup
    execute <| Register someProvider

    // Test
    match getProvider <| getLastId "Profile" with
    | Some p -> ()
    | None   -> Assert.Fail()

[<Test>]
let ``Get platforms`` () =

    getPlatforms() |> List.isEmpty |> should equal false

[<Test>]
let ``Add source`` () =

    //Setup
    execute <| Register someProvider
    let providerId = getLastId "Profile"

    // Test
    execute <| AddSource { someSource with ProfileId= providerId }

    // Verify
    let sourceId = getLastId "Source"
    let sql = @"SELECT Id FROM [dbo].[Source] WHERE Id = @id"
    let (connection,command) = createCommand sql connectionString

    try connection.Open()
        command.Parameters.AddWithValue("@id", sourceId) |> ignore

        use reader = command.ExecuteReader()
        reader.Read() |> should equal true

    // Teardown
    finally dispose connection command

[<Test>]
let ``Get sources`` () =

    //Setup
    execute <| Register someProvider

    let providerId = getLastId "Profile"
    execute <| AddSource { someSource with ProfileId = providerId }

    // Test
    let sources = providerId |> getSources
    
    // Verify
    sources |> List.isEmpty |> should equal false

[<Test>]
let ``Remove source`` () =

    //Setup
    execute <| Register someProvider
    let providerId = getLastId "Profile"

    execute <| AddSource { someSource with ProfileId= providerId }
    let sourceId = getLastId "Source"

    // Test
    execute <| RemoveSource { SourceId = sourceId }

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
    ``Add source`` ()
    0