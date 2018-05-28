module Nikeza.Mobile.Access.Using

open Nikeza.Access.Specification.Attempts
open Nikeza.Access.Specification.Commands
open Logic

module ValidatedForm =

    let attempt : SubmitAttempt =
        fun submit -> function
            Registration.Attach form -> 
                                form |> submit

module Login =
    
    let attempt : LoginAttempt =
        fun login -> function
            Submit credentials -> 
                   credentials |> login

module Logout =
    
    let attempt : LogoutAttempt =
        fun logout -> function
            Logout provider ->
                   provider |> logout