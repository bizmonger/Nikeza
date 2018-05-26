module Nikeza.Mobile.Access.In

open Nikeza.Access.Specification.Commands.Registration
open Nikeza.Access.Specification.Commands.Session
open Nikeza.Access.Specification.Workflows
open Nikeza.Access.Specification.Commands
open Logic

module SubmitRegistration =
    open Submit

    let workflow : SubmitWorkflow =
        fun submitFn -> function
        Registration.Command
                    .Execute form -> 
                             form |> submitFn
                                  |> ResultOf.Submit.Executed
                                  |> Are.Registration.Submission.events

module ValidateRegistration =
    open Registration.Validate
    
    let workflow : ValidateWorkflow = function
        Registration.Validate
                    .Execute form -> 
                             form |> Registration.validate
                                  |> ResultOf.Validate.Executed
                                  |> Are.Registration.Validation.events
                             
module Login =
    
    let workflow : SessionWorkflow =
        fun loginFn -> function
        Submit credentials -> 
               credentials |> loginFn
                           |> ResultOf.Login
                           |> Are.Login.events

module Logout =
    
    let workflow : LogoutWorkflow = function
        Logout p ->
               p |> Try.logout
                 |> ResultOf.Logout
                 |> Are.Logout.events