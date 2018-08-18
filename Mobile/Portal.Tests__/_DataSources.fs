module _DataSources

open System.Linq
open FsUnit
open NUnit.Framework
open Nikeza.Mobile.UILogic.Portal.DataSources
open Nikeza.Mobile.TestAPI


[<Test>]
let ``Initializing viewmodel loads platforms`` () =
    
    // Setup
    let dataSources = ViewModel(DataSource.dependencies)

    // Test
    dataSources.Init()

    // Verify
    dataSources.Platforms.Any() |> should equal true