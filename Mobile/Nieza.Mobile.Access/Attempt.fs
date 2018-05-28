module Nikeza.Mobile.Access.Using

open Nikeza.Access.Specification.Attempts
open Nikeza.Access.Specification.Commands
open Logic

module SubmitRegistration =

    let attempt : SubmitAttempt =
        fun submit -> function
            Registration.Submit
                        .Execute form -> 
                                 form |> submit

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