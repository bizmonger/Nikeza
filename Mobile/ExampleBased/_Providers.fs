module Tests.Providers

open FsUnit
open NUnit.Framework
open Nikeza.Mobile.TestAPI
open Nikeza.Mobile.UILogic.Portal
open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.UILogic.Pages

[<Test>]
let ``Load recent links from providers`` () =
    
    // Setup
    let viewmodel = Recent.ViewModel(Recent.dependencies)
    
    // Test
    viewmodel.Init()

    // Verify
    viewmodel.Providers |> List.isEmpty |> should equal false

[<Test>]
let ``Load members`` () =
    
    // Setup
    let viewmodel = Members.ViewModel(Members.dependencies)
    
    // Test
    viewmodel.Init()

    // Verify
    viewmodel.Providers |> List.isEmpty |> should equal false

[<Test>]
let ``Load subscriptions`` () =
    
    // Setup
    let viewmodel = Subscriptions.ViewModel(Subscriptions.dependencies)
    
    // Test
    viewmodel.Init()

    // Verify
    viewmodel.Providers |> List.isEmpty |> should equal false

[<Test>]
let ``Load portfolio``() =
    
    // Setup
    let viewmodel = Portfolio.ViewModel(Portfolio.dependencies)

    // Test
    viewmodel.Init()

    // Verify
    viewmodel.Provider.IsSome |> should equal true

[<Test>]
let ``Follow provider``() =
    
    // Setup
    let mutable followed = false
    let sideEffect = function SubscriberAdded _ -> followed <- true | _ -> ()

    let sideEffects =  { Portfolio.dependencies.SideEffects with ForFollow=[sideEffect] }
    let dependencies = { Portfolio.dependencies with SideEffects= sideEffects }
    
    // Test
    Portfolio.ViewModel(dependencies).Follow
                                     .Execute()

    // Verify
    followed |> should equal true

[<Test>]
let ``Unsubscribe from provider``() =
    
    // Setup
    let mutable unsubscribed = false
    let sideEffect = function SubscriberRemoved _ -> unsubscribed <- true | _ -> ()

    let sideEffects =  { Portfolio.dependencies.SideEffects with ForUnsubscribe=[sideEffect] }
    let dependencies = { Portfolio.dependencies with SideEffects= sideEffects }
    
    // Test
    Portfolio.ViewModel(dependencies).Unsubscribe
                                     .Execute()

    // Verify
    unsubscribed |> should equal true

[<Test>]
let ``Navigate: portfolio -> articles``() =
    
    // Setup
    let mutable pageRequested = false
    let sideEffect = function PageRequested.Articles _ -> pageRequested <- true | _ -> ()

    let sideEffects =  { Portfolio.dependencies.SideEffects with ForPageRequested=[sideEffect] }
    let dependencies = { Portfolio.dependencies with SideEffects= sideEffects }
    
    // Test
    Portfolio.ViewModel(dependencies).Articles
                                     .Execute()

    // Verify
    pageRequested |> should equal true

[<Test>]
let ``Navigate: portfolio -> videos``() =
    
    // Setup
    let mutable pageRequested = false
    let sideEffect = function PageRequested.Videos _ -> pageRequested <- true | _ -> ()

    let sideEffects =  { Portfolio.dependencies.SideEffects with ForPageRequested=[sideEffect] }
    let dependencies = { Portfolio.dependencies with SideEffects= sideEffects }
    
    // Test
    Portfolio.ViewModel(dependencies).Videos
                                     .Execute()
    // Verify
    pageRequested |> should equal true

[<Test>]
let ``Navigate: portfolio -> answers``() =
    
    // Setup
    let mutable pageRequested = false
    let sideEffect = function PageRequested.Answers _ -> pageRequested <- true | _ -> ()

    let sideEffects =  { Portfolio.dependencies.SideEffects with ForPageRequested=[sideEffect] }
    let dependencies = { Portfolio.dependencies with SideEffects= sideEffects }
    
    // Test
    Portfolio.ViewModel(dependencies).Answers
                                     .Execute()

    // Verify
    pageRequested |> should equal true

[<Test>]
let ``Navigate: portfolio -> podcasts``() =
    
    // Setup
    let mutable pageRequested = false
    let sideEffect = function PageRequested.Podcasts _ -> pageRequested <- true | _ -> ()

    let sideEffects =  { Portfolio.dependencies.SideEffects with ForPageRequested=[sideEffect] }
    let dependencies = { Portfolio.dependencies with SideEffects= sideEffects }
    
    // Test
    Portfolio.ViewModel(dependencies).Podcasts
                                     .Execute()

    // Verify
    pageRequested |> should equal true