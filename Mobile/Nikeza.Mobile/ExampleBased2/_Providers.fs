module Tests.Providers

open FsUnit
open NUnit.Framework
open TestAPI
open Nikeza.Mobile.UILogic.Portal

[<Test>]
let ``Load recent links from providers`` () =
    
    // Setup
    let viewmodel = Recent.ViewModel(someProvider, mockRecent)
    
    // Test
    viewmodel.Load()

    // Verify
    viewmodel.Providers |> List.isEmpty |> should equal false

[<Test>]
let ``Load members`` () =
    
    // Setup
    let viewmodel = Members.ViewModel(someProvider, mockMembers)
    
    // Test
    viewmodel.Load()

    // Verify
    viewmodel.Providers |> List.isEmpty |> should equal false

[<Test>]
let ``Load subscriptions`` () =
    
    // Setup
    let viewmodel = Subscriptions.ViewModel(someProvider, mockSubscriptions)
    
    // Test
    viewmodel.Load()

    // Verify
    viewmodel.Providers |> List.isEmpty |> should equal false