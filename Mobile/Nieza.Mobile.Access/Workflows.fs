module Nikeza.Mobile.Access.For

open Nikeza.Access.Specification.Commands.Session
open Nikeza.Access.Specification.Workflows
open Nikeza.Access.Specification.Attempts
open Nikeza.Access.Specification.Commands
open Logic

module SubmitRegistration =

    let attempt : SubmitAttempt =
        fun submit -> function
            Registration.Command
                        .Execute form -> 
                                 form |> submit

module ValidateRegistration =
    open Registration.Validate
    
    let workflow : ValidateWorkflow = function
        Registration.Validate
                    .Execute form -> 
                             form |> Registration.validate
                                  |> ResultOf.Validate.Executed
                                  |> Are.Registration.Validation.events
                             
module Login =
    
    let attempt : LoginAttempt =
        fun login -> function
            Submit credentials -> 
                   credentials |> login

module Logout =
    
    let attempt : LogoutAttempt =
        fun logout -> function
            Logout p ->
                   p |> logout