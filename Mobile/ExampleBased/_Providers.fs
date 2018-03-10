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

    let response = function 
        | SubscriberAdded _ -> followed <- true 
        | _                 -> followed <- false

    let responders =   { Portfolio.dependencies.Observers with ForFollow=[response] }
    let dependencies = { Portfolio.dependencies with Observers= responders }
    
    let viewmodel = Portfolio.ViewModel(dependencies)

    // Test
    viewmodel.Follow.Execute()

    // Verify
    followed |> should equal true

[<Test>]
let ``Unsubscribe from provider``() =
    
    // Setup
    let mutable unsubscribed = false

    let response = function 
        | SubscriberRemoved _ -> unsubscribed <- true 
        | _                   -> unsubscribed <- false

    let responders =   { Portfolio.dependencies.Observers with ForUnsubscribe=[response] }
    let dependencies = { Portfolio.dependencies with Observers= responders }
    
    let viewmodel = Portfolio.ViewModel(dependencies)

    // Test
    viewmodel.Unsubscribe.Execute()

    // Verify
    unsubscribed |> should equal true

[<Test>]
let ``Navigate: portfolio -> articles``() =
    
    // Setup
    let mutable pageRequested = false

    let response = function 
        | PageRequested.Articles _ -> pageRequested <- true 
        | _                        -> pageRequested <- false

    let responders =   { Portfolio.dependencies.Observers with ForPageRequested=[response] }
    let dependencies = { Portfolio.dependencies with Observers= responders }
    
    let viewmodel = Portfolio.ViewModel(dependencies)
    
    // Test
    viewmodel.Articles.Execute()

    // Verify
    pageRequested |> should equal true

[<Test>]
let ``Navigate: portfolio -> videos``() =
    
    // Setup
    let mutable pageRequested = false
   
    let response = function 
        | PageRequested.Videos _ -> pageRequested <- true 
        | _                      -> pageRequested <- false

    let responders =   { Portfolio.dependencies.Observers with ForPageRequested=[response] }
    let dependencies = { Portfolio.dependencies with Observers= responders }
    
    let viewmodel = Portfolio.ViewModel(dependencies)

    // Test
    viewmodel.Videos.Execute()

    // Verify
    pageRequested |> should equal true

[<Test>]
let ``Navigate: portfolio -> answers``() =
    
    // Setup
    let mutable pageRequested = false
   
    let response = function 
        | PageRequested.Answers _ -> pageRequested <- true 
        | _                       -> pageRequested <- false

    let responders =   { Portfolio.dependencies.Observers with ForPageRequested=[response] }
    let dependencies = { Portfolio.dependencies with Observers= responders }
    
    let viewmodel = Portfolio.ViewModel(dependencies)
    // Test
    viewmodel.Answers.Execute()

    // Verify
    pageRequested |> should equal true

[<Test>]
let ``Navigate: portfolio -> podcasts``() =
    
    // Setup
    let mutable pageRequested = false
   
    let response = function 
        | PageRequested.Podcasts _ -> pageRequested <- true 
        | _                        -> pageRequested <- false
        
    let responders =   { Portfolio.dependencies.Observers with ForPageRequested=[response] }
    let dependencies = { Portfolio.dependencies with Observers= responders }
    
    let viewmodel = Portfolio.ViewModel(dependencies)

    // Test
    viewmodel.Podcasts.Execute()

    // Verify
    pageRequested |> should equal true