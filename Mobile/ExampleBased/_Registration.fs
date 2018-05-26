module Tests.Registration

open FsUnit
open NUnit.Framework
open Nikeza.Mobile.TestAPI
open Nikeza.Mobile.UILogic.Registration
open Nikeza.Access.Specification.Events
open Nikeza.Mobile.AppLogic.RegistrationEvents.Register
open Nikeza.Mobile.TestAPI.Registration
open Xamarin.Forms

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

    let sideEffects =  { ForRegistrationSubmission= [mutateOnSuccess] }
    let dependencies = { dependencies with SideEffects= sideEffects }
    
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
    let sideEffects =  { ForRegistrationSubmission= [] } |> appendNavigation Application.Current
    let dependencies = { dependencies with SideEffects= sideEffects }

    let registration =   ViewModel(dependencies)

    registration.FillOut()
    registration.Validate.Execute()

    // Test
    registration.Submit.Execute()