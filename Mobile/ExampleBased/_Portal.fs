module Tests.Portal

open FsUnit
open NUnit.Framework
open Nikeza.Mobile.TestAPI
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Portal

[<Test>]
let ``Initialize viewmodel`` () =

    // Setup
    let portal = ViewModel(Portal.dependencies)
    
    // Test
    portal.Init()

    // Verify
    portal.Subscriptions.Count |> should equal 3