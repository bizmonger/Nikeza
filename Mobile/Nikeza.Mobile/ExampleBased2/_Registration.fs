module Tests.Registration

open FsUnit
open NUnit.Framework
open Nikeza.Mobile.UILogic.Registration

[<Test>]
let ``Registration succeeds with email and matching passwords`` () =
    
    // Setup
    let viewmodel = ViewModel()
    viewmodel.Email    <- "scott@abc.com"
    viewmodel.Password <- "some_password"
    viewmodel.Confirm  <- "some_password"

    // Test
    viewmodel.Validate.Execute()

    // Verify
    viewmodel.IsValidated |> should equal true