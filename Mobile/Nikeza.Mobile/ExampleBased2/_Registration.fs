module Tests.Registration

open FsUnit
open NUnit.Framework
open Nikeza.Mobile.UILogic.Registration
open TestAPI
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
    let mutable registrationSucceeded = false

    let registration = ViewModel(mockSubmit)

    registration.FillIn()
                .EventOccured
                .Add(fun event -> 
                         event |> function
                                  | RegistrationSucceeded _ -> registrationSucceeded <- true
                                  | RegistrationFailed    _ -> registrationSucceeded <- false)

    registration.Validate.Execute()

    // Test
    registration.Submit.Execute()

    // Verify
    registrationSucceeded |> should equal true