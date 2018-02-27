module Tests.Registration

open FsUnit
open NUnit.Framework
open Nikeza.Mobile.TestAPI
open Nikeza.Mobile.UILogic.Registration
open Nikeza.Mobile.Profile.Events

[<Test>]
let ``Registration validated with email and matching passwords`` () =
    
    // Setup
    let registration = ViewModel(mockSubmit, fun _ -> ())
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
    let mutable successful = false

    let mutateOnSuccess = function 
        | RegistrationSucceeded _ -> successful <- true 
        | _                       -> successful <- false

    let hasRegistrationSucceeded event = event |> mutateOnSuccess

    let handler events = events |> List.iter hasRegistrationSucceeded
    let registration =   ViewModel(mockSubmit, handler)

    registration.FillOut()
    registration.Validate.Execute()

    // Test
    registration.Submit.Execute()

    // Verify
    successful |> should equal true