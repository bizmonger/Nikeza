module Integration

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
    let data = { FollowRequest.SubscriberId = 0; FollowRequest.ProviderId=0 }

    // Test
    execute <| Follow data

    // Verify
    let sql = @"SELECT ProfileId, ProviderId
                FROM   [dbo].[Subscription]
                WHERE  ProfileId  = @ProfileId
                AND    ProviderId = @ProviderId"

    let (connection,command) = createCommand(sql)

    command.Parameters.AddWithValue("@ProfileId",  data.SubscriberId) |> ignore
    command.Parameters.AddWithValue("@ProviderId", data.ProviderId)   |> ignore

    use reader = command |> prepareReader
    let entryAdded = reader.GetInt32(0) = 0 && reader.GetInt32(1) = 0

    entryAdded |> should equal true

    // Teardown
    dispose connection command

[<Test>]
let ``Unsubscribe`` () =
    // Setup
    let data = { SubscriberId = 0; ProviderId=0 }

    // Test
    execute <| Unsubscribe data

    // Verify
    let sql = @"SELECT ProfileId, ProviderId
                FROM   [dbo].[Subscription]
                WHERE  ProfileId  = @ProfileId
                AND    ProviderId = @ProviderId"

    let (connection,command) = createCommand(sql)

    command.Parameters.AddWithValue("@ProfileId",  data.SubscriberId) |> ignore
    command.Parameters.AddWithValue("@ProviderId", data.ProviderId)   |> ignore

    let reader = command.ExecuteReader()
    reader.Read() |> should equal false

    // Teardown
    dispose connection command

[<Test>]
let ``Feature Link`` () =
    //Setup
    let data = { LinkId = 0; IsFeatured = true }

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
    let data = { LinkId = 0; IsFeatured = false }

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

// [<Test>]
// let ``Registration`` () = ()
//     // Setup

//     // Test

//     // Verify

//     // Teardown

// [<Test>]
// let ``Signin`` () = ()
//     // Setup

//     // Test

//     // Verify

//     // Teardown

// [<Test>]
// let ``Update profile`` () = ()
//     // Setup

//     // Test

//     // Verify

//     // Teardown