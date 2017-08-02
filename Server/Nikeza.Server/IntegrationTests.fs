module Integration

open FsUnit
open NUnit.Framework
open Nikeza.Server.Models
open Nikeza.Server.DataAccess

[<Test>]
let ``Follow`` () =
    // Test
    execute <| Follow { SubscriberId = 0; ProviderId=0 }

    // Verify
    (*Assert entry in datastore*)

[<Test>]
let ``Unsubscribe`` () =
    // Test
    execute <| Unsubscribe { SubscriberId = 0; ProviderId=0 }

    // Verify

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

    // 

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