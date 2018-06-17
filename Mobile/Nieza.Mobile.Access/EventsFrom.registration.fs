namespace Nikeza.Mobile.Access

open Nikeza.Common
open Nikeza.Access.Specification.Events

module Submission =
    open Nikeza.Access.Specification.Submission

    let toEvents : RegistrationSubmission = function
        
        result -> 
        result |> function
                  | Ok    profile -> { Head= RegistrationSucceeded profile; Tail=[] }
                  | Error form    -> { Head= RegistrationFailed    form;    Tail=[] }

module internal Validation =
    open Nikeza.Access.Specification.Validation

    let toEevents : RegistrationValidation = function
        
        result -> 
        result |> function
                  | Error form -> { Head= FormNotValidated form; Tail=[] }
                  | Ok    form -> { Head= FormValidated    form; Tail=[] }