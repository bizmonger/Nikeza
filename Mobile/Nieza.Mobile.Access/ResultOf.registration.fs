namespace Are

open Nikeza.Common
open Nikeza.Access.Specification.Events

module Submission =
    open Nikeza.Access.Specification.Commands.Registration.Submit
    open Nikeza.Access.Specification.Submission

    let events : RegistrationSubmission = function
        ResultOf.Submit
                .Executed result -> 
                          result |> function
                                    | Ok    profile -> { Head= RegistrationSucceeded profile; Tail=[] }
                                    | Error form    -> { Head= RegistrationFailed    form;    Tail=[] }

module internal Validation =
    open Nikeza.Access.Specification.Commands.Registration.Validate
    open Nikeza.Access.Specification.Validation

    let events : RegistrationValidation = function
        ResultOf.Validate
                .Executed result -> 
                          result |> function
                                    | Error form -> { Head= FormNotValidated form; Tail=[] }
                                    | Ok    form -> { Head= FormValidated    form; Tail=[] }