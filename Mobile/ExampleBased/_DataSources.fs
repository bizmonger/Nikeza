module _DataSources

open FsUnit
open NUnit.Framework
open Nikeza.Mobile.Portal.DataSources
open Nikeza.Mobile.TestAPI
open System.Linq

[<Test>]
let ``Initializing viewmodel loads platforms`` () =
    
    // Setup
    let dataSources = ViewModel(DataSource.dependencies)

    // Test
    dataSources.Init()

    // Verify
    dataSources.Platforms.Any() |> should equal true