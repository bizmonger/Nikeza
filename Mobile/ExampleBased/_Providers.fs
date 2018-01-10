module Tests.Providers

open FsUnit
open NUnit.Framework
open Nikeza.Mobile.TestAPI
open Nikeza.Mobile.TestAPI.Portfolio
open Nikeza.Mobile.UILogic.Portal
open Nikeza.Mobile.UILogic.Pages
open Nikeza.Mobile.Subscriptions.Events

[<Test>]
let ``Load recent links from providers`` () =
    
    // Setup
    let viewmodel = Recent.ViewModel(someProvider, mockRecent, mockPortfolio)
    
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

[<Test>]
let ``Load portfolio``() =
    
    // Setup
    let viewmodel = Portfolio.ViewModel(injected)

    // Test
    viewmodel.Load()

    // Verify
    viewmodel.Provider.IsSome |> should equal true

[<Test>]
let ``Follow provider``() =
    
    // Setup
    let mutable followSucceeded = false
    
    let viewmodel = Portfolio.ViewModel(injected)

    viewmodel.CommandEvents()
             .Add(fun event ->
                      event |> function 
                               | SubscriberAdded _ -> followSucceeded <- true
                               | _ ->                 followSucceeded <- false)
    // Test
    viewmodel.Follow.Execute()

    // Verify
    followSucceeded |> should equal true

[<Test>]
let ``Unsubscribe from provider``() =
    
    // Setup
    let mutable unsubscribeSucceeded = false
   
    let viewmodel = Portfolio.ViewModel(injected)

    viewmodel.CommandEvents()
             .Add(fun event ->
                      event |> function 
                               | SubscriberRemoved _ -> unsubscribeSucceeded <- true
                               | _ ->                   unsubscribeSucceeded <- false)
    // Test
    viewmodel.Unsubscribe.Execute()

    // Verify
    unsubscribeSucceeded |> should equal true

[<Test>]
let ``Navigate: portfolio -> articles``() =
    
    // Setup
    let mutable pageRequested = false
   
    let viewmodel = Portfolio.ViewModel(injected)

    viewmodel.PageRequested()
             .Add(fun event ->
                      event |> function 
                               | PageRequested.Articles _ -> pageRequested <- true
                               | _ ->                        pageRequested <- false)
    // Test
    viewmodel.Articles.Execute()

    // Verify
    pageRequested |> should equal true

[<Test>]
let ``Navigate: portfolio -> videos``() =
    
    // Setup
    let mutable pageRequested = false
   
    let viewmodel = Portfolio.ViewModel(injected)

    viewmodel.PageRequested()
             .Add(fun event ->
                      event |> function 
                               | PageRequested.Videos _ -> pageRequested <- true
                               | _ ->                      pageRequested <- false)
    // Test
    viewmodel.Videos.Execute()

    // Verify
    pageRequested |> should equal true

[<Test>]
let ``Navigate: portfolio -> answers``() =
    
    // Setup
    let mutable pageRequested = false
   
    let viewmodel = Portfolio.ViewModel(injected)

    viewmodel.PageRequested()
             .Add(fun event ->
                      event |> function 
                               | PageRequested.Answers _ -> pageRequested <- true
                               | _ ->                       pageRequested <- false)
    // Test
    viewmodel.Answers.Execute()

    // Verify
    pageRequested |> should equal true

[<Test>]
let ``Navigate: portfolio -> podcasts``() =
    
    // Setup
    let mutable pageRequested = false
   
    let viewmodel = Portfolio.ViewModel(injected)

    viewmodel.PageRequested()
             .Add(fun event ->
                      event |> function 
                               | PageRequested.Podcasts _ -> pageRequested <- true
                               | _ ->                        pageRequested <- false)
    // Test
    viewmodel.Podcasts.Execute()

    // Verify
    pageRequested |> should equal true