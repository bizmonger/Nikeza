module internal Are.Registration

open Nikeza.Common
open Nikeza.Access.Specification.Events

module Submission =
    open Nikeza.Access.Specification.Commands.Registration.Submit

    type private RegistrationSubmission = ResultOf.Submit -> RegistrationSubmissionEvent nonempty

    let events : RegistrationSubmission = function
        ResultOf.Submit.Executed result -> 
                                 result |> function
                                           | Ok    profile -> { Head= RegistrationSucceeded profile; Tail=[] }
                                           | Error form    -> { Head= RegistrationFailed    form;    Tail=[] }

module Validation =
    open Nikeza.Access.Specification.Commands.Registration.Validate

    type private RegistrationValidation = ResultOf.Validate -> RegistrationValidationEvent nonempty

    let events : RegistrationValidation = function
        ResultOf.Validate.Executed result -> 
                                   result |> function
                                             | Error form -> { Head= FormNotValidated form; Tail=[] }
                                             | Ok    form -> { Head= FormValidated    form; Tail=[] }