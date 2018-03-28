﻿module Tests.Portal

open FsUnit
open NUnit.Framework
open Nikeza.Mobile.TestAPI
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Portal
open Nikeza.Mobile.UILogic.Pages
open Nikeza.Mobile.AppLogic
open System.Linq

[<Test>]
let ``Initialize viewmodel`` () =

    // Setup
    let portal = ViewModel(Portal.dependencies)
    
    // Test
    portal.Init()

    // Verify
    portal.Subscriptions.Any() |> should equal true

[<Test>]
let ``Navigate: Portal to Members`` () =

    // Setup
    let mutable pageRequested = false
    let sideEffect = function PageRequested.Members _ -> pageRequested <- true | _ -> ()

    let sideEffects =  { Portal.dependencies.SideEffects with ForPageRequested=[sideEffect] }
    let dependencies = { Portal.dependencies with SideEffects= sideEffects }

    let portal = ViewModel(dependencies)
    portal.Init()

    // Test
    portal.ViewMembers.Execute()
    
    // Verify
    pageRequested |> should equal true

[<Test>]
let ``Navigate: Portal to Latest`` () =

    // Setup
    let mutable pageRequested = false
    let sideEffect = function PageRequested.Latest _ -> pageRequested <- true | _ -> ()

    let sideEffects =  { Portal.dependencies.SideEffects with ForPageRequested=[sideEffect] }
    let dependencies = { Portal.dependencies with SideEffects= sideEffects }

    let portal = ViewModel(dependencies)
    portal.Init()

    // Test
    portal.ViewLatest.Execute()
    
    // Verify
    pageRequested |> should equal true

[<Test>]
let ``Navigate: Portal to Followers`` () =

    // Setup
    let mutable pageRequested = false
    let sideEffect = function PageRequested.Followers _ -> pageRequested <- true | _ -> ()

    let sideEffects =  { Portal.dependencies.SideEffects with ForPageRequested=[sideEffect] }
    let dependencies = { Portal.dependencies with SideEffects= sideEffects }

    let portal = ViewModel(dependencies)
    portal.Init()

    // Test
    portal.ViewFollowers.Execute()
    
    // Verify
    pageRequested |> should equal true

[<Test>]
let ``Navigate: Portal to Subscriptions`` () =

    // Setup
    let mutable pageRequested = false
    let sideEffect = function PageRequested.Subscriptions _ -> pageRequested <- true | _ -> ()

    let sideEffects =  { Portal.dependencies.SideEffects with ForPageRequested=[sideEffect] }
    let dependencies = { Portal.dependencies with SideEffects= sideEffects }

    let portal = ViewModel(dependencies)
    portal.Init()

    // Test
    portal.ViewSubscriptions.Execute()
    
    // Verify
    pageRequested |> should equal true