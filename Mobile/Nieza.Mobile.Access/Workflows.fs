module Nikeza.Mobile.Access.In

open Nikeza.Mobile.Access.Commands
open Nikeza.Mobile.Access.Commands.Registration
open Nikeza.Mobile.Access.Commands.Session
open Nikeza.Mobile.Access.Events
open Logic
open Try


module SubmitRegistration =
    open Submit

    type private SubmitWorkflow = SubmitFn -> Command -> RegistrationSubmissionEvent list

    let workflow : SubmitWorkflow =
        fun submitFn -> function
        Command.Execute form -> 
                        form |> submitFn
                             |> ResultOf.Submit.Executed
                             |> Are.Registration.Submission.events

module ValidateRegistration =
    open Registration.Validate

    type private ValidateWorkflow = Validate -> RegistrationValidationEvent list

    let workflow : ValidateWorkflow = function
        Validate.Execute form -> 
                         form |> Registration.validate
                              |> ResultOf.Validate.Executed
                              |> Are.Registration.Validation.events

module Login =
    type private SessionWorkflow = LoginFn -> LoginCommand -> LoginEvent list

    let workflow : SessionWorkflow =
        fun loginFn -> function
        Submit credentials -> 
              credentials |> loginFn
                          |> ResultOf.Login
                          |> Are.Login.events

module Logout =
    type private LogoutWorkflow = LogoutCommand -> LogoutEvent list

    let workflow : LogoutWorkflow = function
        Logout p ->
               p |> Try.logout
                 |> ResultOf.Logout
                 |> Are.Logout.events