module Tests.Registration

open FsUnit
open NUnit.Framework
open Nikeza.Mobile.TestAPI
open Nikeza.Mobile.UILogic.Registration
open Nikeza.Mobile.Profile.Events
open Nikeza.Mobile.AppLogic.ProfileEvents.Register
open Nikeza.Mobile.TestAPI.Registration

[<Test>]
let ``Registration validated with email and matching passwords`` () =
    
    // Setup
    let registration = ViewModel(dependencies)
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
    let mutateOnSuccess = function RegistrationSucceeded _ -> successful <- true | _ -> ()

    let responders =   { ForRegistrationSubmission= [mutateOnSuccess] }
    let dependencies = { dependencies with SideEffects= responders }
    
    let registration =   ViewModel(dependencies)

    registration.FillOut()
    registration.Validate.Execute()

    // Test
    registration.Submit.Execute()

    // Verify
    successful |> should equal true

[<Test>]
let ``Navigation requested after registration submitted`` () =
    
    // Setup
    let responders =     addTo { ForRegistrationSubmission= [] }
    let dependencies = { dependencies with SideEffects= responders }

    let registration =   ViewModel(dependencies)

    registration.FillOut()
    registration.Validate.Execute()

    // Test
    registration.Submit.Execute()