module Tests.Registration

open FsUnit
open NUnit.Framework
open Nikeza.Mobile.TestAPI
open Nikeza.Mobile.UILogic.Registration
open Nikeza.Mobile.Profile.Events

[<Test>]
let ``Registration validated with email and matching passwords`` () =
    
    // Setup
    let registration = ViewModel(mockSubmit)
    registration.Email    <- someEmail
    registration.Password <- somePassword
    registration.Confirm  <- somePassword

    // Test
    registration.Validate.Execute()

    // Verify
    registration.IsValidated |> should equal true

[<Test>]
let ``Registration submitted after being validated`` () =
    
    // Setup
    let registration =   ViewModel(mockSubmit)
    let succeeded event = event |> function RegistrationSucceeded _ -> true | _ -> false

    registration.FillIn()
    registration.Validate.Execute()

    // Test
    registration.Submit.Execute()

    // Verify
    registration.Events 
     |> List.exists succeeded
     |> should equal true