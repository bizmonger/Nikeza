module Tests.Registration

open FsUnit
open NUnit.Framework
open Nikeza.Mobile.UILogic.Registration
open TestAPI
open Nikeza.Mobile.Profile.Events

[<Test>]
let ``Registration validated with email and matching passwords`` () =
    
    // Setup
    let registration = ViewModel()
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
    let mutable eventOccurred = false

    let registration = ViewModel()
    Apply.valuesTo registration

    registration.EventOccured.Add(fun event -> 
                                      event |> function
                                               | RegistrationSucceeded _ -> eventOccurred <- true
                                               | _                       -> eventOccurred <- false)
    registration.Validate.Execute()

    // Test
    registration.Submit.Execute()

    // Verify
    eventOccurred |> should equal true