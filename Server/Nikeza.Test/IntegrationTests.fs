module Integration

open System
open FsUnit
open NUnit.Framework
open Nikeza.Server.Models
open Nikeza.Server.DataAccess
open System.Data.SqlClient
open Nikeza.TestAPI

// https://github.com/nunit/dotnet-test-nunit

[<Test>]
let ``Follow`` () =

    // Setup
    execute <| Register someProfile
    let data = { FollowRequest.SubscriberId=someSubscriberId; FollowRequest.ProviderId=someProviderId }

    // Test
    execute <| Follow data

    // Verify
    let sql = @"SELECT SubscriberId, ProviderId
                FROM   [dbo].[Subscription]
                WHERE  SubscriberId = @SubscriberId
                AND    ProviderId =   @ProviderId"

    let (connection,command) = createCommand(sql)

    command.Parameters.AddWithValue("@SubscriberId", data.SubscriberId) |> ignore
    command.Parameters.AddWithValue("@ProviderId",   data.ProviderId)   |> ignore

    use reader = command |> prepareReader
    let entryAdded = reader.GetInt32(0) = someSubscriberId && reader.GetInt32(1) = someProviderId

    entryAdded |> should equal true

    // Teardown
    dispose connection command

[<Test>]
let ``Unsubscribe`` () =

    // Setup
    execute <| Register someProfile
    let data = { SubscriberId=someSubscriberId; ProviderId=someProviderId }

    // Test
    execute <| Unsubscribe data

    // Verify
    let sql = @"SELECT SubscriberId, ProviderId
                FROM   [dbo].[Subscription]
                WHERE  SubscriberId = @SubscriberId
                AND    ProviderId =   @ProviderId"

    let (connection,command) = createCommand(sql)

    command.Parameters.AddWithValue("@SubscriberId", data.SubscriberId) |> ignore
    command.Parameters.AddWithValue("@ProviderId",   data.ProviderId)   |> ignore

    let reader = command.ExecuteReader()
    reader.Read() |> should equal false

    // Teardown
    dispose connection command

[<Test>]
let ``Feature Link`` () =

    //Setup
    execute <| Register someProfile
    let data = { LinkId=someLinkId; IsFeatured=true }

    // Test
    execute <| FeatureLink data

    // Verify
    let sql = @"SELECT Id, IsFeatured
                FROM   [dbo].[Link]
                WHERE  Id  = @id
                AND    IsFeatured = @isFeatured"

    let (connection,command) = createCommand(sql)

    command.Parameters.AddWithValue("@id",         data.LinkId)     |> ignore
    command.Parameters.AddWithValue("@isFeatured", data.IsFeatured) |> ignore

    let reader = command.ExecuteReader()
    let isFeatured = reader.GetBoolean(6)
    isFeatured |> should equal true

    // Teardown
    dispose connection command

[<Test>]
let ``Unfeature Link`` () =
    
    //Setup
    execute <| Register someProfile
    let data = { LinkId=someLinkId; IsFeatured=false }

    // Test
    execute <| FeatureLink data

    // Verify
    let sql = @"SELECT Id, IsFeatured
                FROM   [dbo].[Link]
                WHERE  Id  = @id
                AND    IsFeatured = @IsFeatured"

    let (connection,command) = createCommand(sql)

    command.Parameters.AddWithValue("@id",      data.LinkId)     |> ignore
    command.Parameters.AddWithValue("@Enabled", data.IsFeatured) |> ignore

    let reader = command.ExecuteReader()
    let isFeatured = reader.GetBoolean(6)
    isFeatured |> should equal false

    // Teardown
    dispose connection command

[<Test>]
let ``Registration`` () =

    // Setup
    let data = { someProfile with FirstName="Scott"; LastName="Nimrod" }

    // Test
    execute <| Register data

    // Verify
    let sql = @"SELECT (FirstName, LastName)
                FROM   [dbo].[Profile]
                WHERE  FirstName = @Scott
                AND    LastName  = @Nimrod"

    let (connection,command) = createCommand(sql)
    command.Parameters.AddWithValue("@FirstName", data.FirstName) |> ignore
    command.Parameters.AddWithValue("@LastName",  data.LastName)  |> ignore
    
    let reader = command.ExecuteReader()
    let isRegistered = (data.FirstName, data.LastName) = (reader.GetString(0), reader.GetString(1))
    isRegistered |> should equal true

    // Teardown
    dispose connection command

[<Test>]
let ``Update profile`` () =
    
    // Setup
    let data = { someProfile with FirstName="Scott"; LastName="Nimrod" }
    execute <| Register data
    
    // Test
    let sql = @"Update [dbo].[Profile]
                Set    [dbo].[FirstName] = 'MODIFIED_NAME'
                WHERE  FirstName = @Scott
                AND    LastName  = @Nimrod"

    let (connection,command) = createCommand(sql)
    command.Parameters.AddWithValue("@FirstName", data.FirstName) |> ignore
    command.Parameters.AddWithValue("@LastName",  data.LastName)  |> ignore
    command.ExecuteNonQuery() |> ignore

    // Verify
    let sql = @"SELECT (FirstName)
                FROM   [dbo].[Profile]
                WHERE  FirstName = @Scott"

    let (connection,readCommand) = createCommand(sql)
    readCommand.Parameters.AddWithValue("@FirstName", "MODIFIED_NAME") |> ignore
    
    use reader = readCommand |> prepareReader
    reader.GetString(0) = "MODIFIED_NAME" |> should equal true

    // Teardown
    dispose connection command

// [<Test>]
// let ``Signin`` () = ()
//     // Setup

//     // Test

//     // Verify

//     // Teardown