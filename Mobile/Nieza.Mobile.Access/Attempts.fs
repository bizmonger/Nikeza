module Nikeza.Mobile.Access.Using

open Nikeza.Access.Specification.Attempts
open Nikeza.Access.Specification.Commands
open Logic

module SubmitRegistration =

    let interpreter : SubmitAttempt =
        fun submit -> function
            Registration.Submit
                        .Execute form -> 
                                 form |> submit

module Login =
    
    let interpreter : LoginAttempt =
        fun login -> function
            Submit credentials -> 
                   credentials |> login

module Logout =
    
    let interpreter : LogoutAttempt =
        fun logout -> function
            Logout p ->
                   p |> logout