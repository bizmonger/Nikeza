module Tests.Portal

open FsUnit
open NUnit.Framework
open Nikeza.Mobile.TestAPI
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Portal
open Nikeza.Mobile.UILogic.Pages

[<Test>]
let ``Initialize viewmodel`` () =

    // Setup
    let portal = ViewModel(Portal.dependencies)
    
    // Test
    portal.Init()

    // Verify
    portal.Subscriptions.Count |> should equal 3

[<Test>]
let ``Navigate: Portal to Members`` () =

    // Setup
    let portal = ViewModel(Portal.dependencies)
    portal.Init()

    let mutable pageRequested = false
    let sideEffect = function PageRequested.Articles _ -> pageRequested <- true | _ -> ()

    let sideEffects =  { Portal.dependencies.SideEffects with ForPageRequested=[sideEffect] }

    // Test
    portal.ViewMembers.Execute()
    
    // Verify
    pageRequested |> should equal true

[<Test>]
let ``Navigate: Portal to Recent`` () =

    // Setup
    let portal = ViewModel(Portal.dependencies)
    portal.Init()

    let mutable pageRequested = false
    let sideEffect = function PageRequested.Articles _ -> pageRequested <- true | _ -> ()

    // Test
    portal.ViewRecent.Execute()
    

    // Verify
    failwith "todo..."

[<Test>]
let ``Navigate: Portal to Followers`` () =

    // Setup
    let portal = ViewModel(Portal.dependencies)
    portal.Init()

    let mutable pageRequested = false
    let sideEffect = function PageRequested.Articles _ -> pageRequested <- true | _ -> ()

    // Test
    portal.ViewFollowers.Execute()
    

    // Verify
    failwith "todo..."

[<Test>]
let ``Navigate: Portal to Subscriptions`` () =

    // Setup
    let portal = ViewModel(Portal.dependencies)
    portal.Init()

    let mutable pageRequested = false
    let sideEffect = function PageRequested.Articles _ -> pageRequested <- true | _ -> ()

    // Test
    portal.ViewSubscriptions.Execute()
    

    // Verify
    failwith "todo..."