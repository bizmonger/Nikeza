module Nikeza.Mobile.Access.In

open Nikeza.Common
open Nikeza.Access.Specification.Commands
open Nikeza.Access.Specification.Commands.Registration
open Nikeza.Access.Specification.Commands.Session
open Nikeza.Access.Specification.Functions
open Logic

module SubmitRegistration =
    open Submit

    let workflow : SubmitWorkflow =
        fun submitFn -> function
        Command.Execute form -> 
                        form |> submitFn
                             |> ResultOf.Submit.Executed
                             |> Are.Registration.Submission.events

module ValidateRegistration =
    open Registration.Validate
    open Nikeza.Access.Specification.Events

    type private ValidateWorkflow = Validate -> RegistrationValidationEvent nonempty

    let workflow : ValidateWorkflow = function
        Validate.Execute form -> 
                         form |> Registration.validate
                              |> ResultOf.Validate.Executed
                              |> Are.Registration.Validation.events

module Login =
    open Nikeza.Access.Specification.Events

    type private SessionWorkflow = LoginFn -> LoginCommand -> LoginEvent nonempty

    let workflow : SessionWorkflow =
        fun loginFn -> function
        Submit credentials -> 
               credentials |> loginFn
                           |> ResultOf.Login
                           |> Are.Login.events

module Logout =
    open Nikeza.Access.Specification.Events

    type private LogoutWorkflow = LogoutCommand -> LogoutEvent nonempty

    let workflow : LogoutWorkflow = function
        Logout p ->
               p |> Try.logout
                 |> ResultOf.Logout
                 |> Are.Logout.events