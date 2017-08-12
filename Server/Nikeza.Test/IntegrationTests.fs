module Integration

open System
open FsUnit
open NUnit.Framework
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
    DataStore.execute <| Register someProvider
    let providerId =   getLastId "Profile"
    
    DataStore.execute <| Register someSubscriber
    let subscriberId = getLastId "Profile"

    // Test
    DataStore.execute <| Follow { FollowRequest.ProviderId= providerId; FollowRequest.SubscriberId= subscriberId }

    // Verify
    let sql = @"SELECT SubscriberId, ProviderId
                FROM   [dbo].[Subscription]
                WHERE  SubscriberId = @SubscriberId
                AND    ProviderId =   @ProviderId"

    let (connection,command) = createCommand(sql)

    command.Parameters.AddWithValue("@SubscriberId", subscriberId) |> ignore
    command.Parameters.AddWithValue("@ProviderId",   providerId)   |> ignore

    use reader = command |> prepareReader
    let entryAdded = reader.GetInt32(0) = subscriberId && 
                     reader.GetInt32(1) = providerId

    entryAdded |> should equal true

    // Teardown
    dispose connection command


[<Test>]
let ``Unsubscribe from Provider`` () =

    // Setup
    DataStore.execute <| Register someProvider
    let providerId =   getLastId "Profile"
    
    DataStore.execute <| Register someSubscriber
    let subscriberId = getLastId "Profile"

    DataStore.execute <| Follow { FollowRequest.ProviderId= providerId; FollowRequest.SubscriberId= subscriberId }

    // Test
    DataStore.execute <| Unsubscribe { UnsubscribeRequest.SubscriberId= subscriberId; UnsubscribeRequest.ProviderId= providerId }

    // Verify
    let sql = @"SELECT SubscriberId, ProviderId
                FROM   [dbo].[Subscription]
                WHERE  SubscriberId = @SubscriberId
                AND    ProviderId =   @ProviderId"

    let (connection,command) = createCommand(sql)

    command.Parameters.AddWithValue("@SubscriberId", subscriberId) |> ignore
    command.Parameters.AddWithValue("@ProviderId",   providerId)   |> ignore

    use reader = command.ExecuteReader()
    reader.Read() |> should equal false

    // Teardown
    dispose connection command

[<Test>]
let ``Add featured link`` () =

    //Setup
    DataStore.execute <| Register someProvider
    DataStore.execute <| AddLink  someLink

    let lastId =  getLastId "Link"
    let data =  { LinkId=lastId; IsFeatured=true }

    // Test
    DataStore.execute <| FeatureLink data

    // Verify
    let sql = @"SELECT Id, IsFeatured
                FROM   [dbo].[Link]
                WHERE  Id  = @id
                AND    IsFeatured = @isFeatured"

    let (connection,command) = createCommand(sql)

    command.Parameters.AddWithValue("@id",         data.LinkId)     |> ignore
    command.Parameters.AddWithValue("@isFeatured", data.IsFeatured) |> ignore

    use reader = command |> prepareReader
    let isFeatured = reader.GetBoolean(1)
    isFeatured |> should equal true

    // Teardown
    dispose connection command

[<Test>]
let ``Unfeature Link`` () =
    
    //Setup
    DataStore.execute <| Register someProvider
    DataStore.execute <| AddLink  someLink

    let lastId =  getLastId "Link"
    let data =  { LinkId=lastId; IsFeatured=false }

    // Test
    DataStore.execute <| FeatureLink data

    // Verify
    let sql = @"SELECT Id, IsFeatured
                FROM   [dbo].[Link]
                WHERE  Id  = @id
                AND    IsFeatured = @isFeatured"

    let (connection,command) = createCommand(sql)

    command.Parameters.AddWithValue("@id",         data.LinkId)     |> ignore
    command.Parameters.AddWithValue("@isFeatured", data.IsFeatured) |> ignore

    use reader = command |> prepareReader
    let isFeatured = reader.GetBoolean(1)
    isFeatured |> should equal false

    // Teardown
    dispose connection command

[<Test>]
let ``Register Profile`` () =

    // Setup
    let data = { someProvider with FirstName="Scott"; LastName="Nimrod" }

    // Test
    DataStore.execute <| Register data

    // Verify
    let sql = @"SELECT FirstName, LastName
                FROM   [dbo].[Profile]
                WHERE  FirstName = @FirstName
                AND    LastName  = @LastName"

    let (connection,command) = createCommand(sql)
    command.Parameters.AddWithValue("@FirstName", data.FirstName) |> ignore
    command.Parameters.AddWithValue("@LastName",  data.LastName)  |> ignore
    
    use reader = command |> prepareReader
    let isRegistered = (data.FirstName, data.LastName) = (reader.GetString(0), reader.GetString(1))
    isRegistered |> should equal true

    // Teardown
    dispose connection command

[<Test>]
let ``Update profile`` () =
    
    // Setup
    let modifiedName = "MODIFIED_NAME"
    let data = { someProvider with FirstName="Scott"; LastName="Nimrod" }
    DataStore.execute <| Register data

    let lastId =  getLastId "Profile"

    // Test
    DataStore.execute <| UpdateProfile { ProviderId = lastId
                                         FirstName =  data.FirstName
                                         LastName =   modifiedName
                                         Bio =        data.Bio
                                         Email =      data.Email }
    // Verify
    let sql = @"SELECT LastName FROM   [dbo].[Profile] WHERE  Id = @Id"
    let (readConnection,readCommand) = createCommand(sql)
    readCommand.Parameters.AddWithValue("@Id", lastId) |> ignore
    
    use reader = readCommand |> prepareReader
    reader.GetString(0) = modifiedName |> should equal true

    // Teardown
    dispose readConnection readCommand

[<Test>]
let ``get links`` () =

    //Setup
    DataStore.execute <| Register someProvider
    DataStore.execute <| AddLink  someLink

    let request = GetLinks { ProviderId = someProvider.ProfileId }

    // Test
    let links = respondTo request

    // Verify
    let linkFound = links |> Seq.head
    linkFound.ProviderId  |> should equal someProvider.ProfileId


// [<Test>]
// let ``Signin`` () = ()
//     // Setup

//     // Test

//     // Verify

//     // Teardown


[<EntryPoint>]
let main argv =
    cleanDataStore()                      
    ``get links`` ()
    0