module Tests.Recent

open FsUnit
open NUnit.Framework
open Nikeza.Mobile.UILogic.Portal.Recent
open TestAPI

[<Test>]
let ``Load recent links from providers`` () =
    
    // Setup
    let latest = ViewModel(someProvider, mockRecent)
    
    // Test
    latest.Load()

    // Verify
    latest.Providers |> List.isEmpty |> should equal false