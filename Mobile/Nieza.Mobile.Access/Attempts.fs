module Nikeza.Mobile.Access.Using

open Nikeza.Access.Specification.Attempts
open Nikeza.Access.Specification.Commands
open Logic

module SubmitRegistration =

    let implementation : SubmitAttempt =
        fun submit -> function
            Registration.Submit
                        .Execute form -> 
                                 form |> submit

module Login =
    
    let implementation : LoginAttempt =
        fun login -> function
            Submit credentials -> 
                   credentials |> login

module Logout =
    
    let implementation : LogoutAttempt =
        fun logout -> function
            Logout p ->
                   p |> logout