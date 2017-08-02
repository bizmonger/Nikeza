module Integration

open FsUnit
open NUnit.Framework
open Nikeza.Server.Models
open Nikeza.Server.DataAccess
open System.Data.SqlClient

let createCommand sql =
    use connection = new SqlConnection(ConnectionString)
    new SqlCommand(sql,connection)

open System.Data.SqlClient
let prepareReader (command:SqlCommand) =
    let reader = command.ExecuteReader()
    reader.Read() |> ignore
    reader

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

    use command = createCommand(sql)
    command.Parameters.AddWithValue("@ProfileId",  data.SubscriberId) |> ignore
    command.Parameters.AddWithValue("@ProviderId", data.ProviderId)   |> ignore

    use reader = command |> prepareReader
    let entryAdded = reader.GetInt32(0) = 0 && reader.GetInt32(1) = 0

    entryAdded |> should equal true

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

    use command = createCommand(sql)
    command.Parameters.AddWithValue("@ProfileId",  data.SubscriberId) |> ignore
    command.Parameters.AddWithValue("@ProviderId", data.ProviderId)   |> ignore

    let reader = command.ExecuteReader()
    reader.Read() |> should equal false

[<Test>]
let ``Feature Link`` () =
    // Test
    execute <| FeatureLink { LinkId = 0; Enabled=true }

    // Verify
    (*Assert entry in datastore*)

[<Test>]
let ``Unfeature Link`` () =
    // Test
    execute <| FeatureLink { LinkId = 0; Enabled=false }

    // Verify
    (*Assert entry in datastore*)

[<Test>]
let ``Registration`` () = ()
    // Setup

    // Test

    // Verify

[<Test>]
let ``Signin`` () = ()
    // Setup

    // Test

    // Verify

[<Test>]
let ``Update profile`` () = ()
    // Setup

    // Test

    // Verify