module internal Are.Registration

open Nikeza.Common
open Nikeza.Mobile.Access.Events

module Submission =
    open Nikeza.Mobile.Access.Commands.Registration.Submit

    type private RegistrationSubmission = ResultOf.Submit -> RegistrationSubmissionEvent nonempty

    let events : RegistrationSubmission = function
        ResultOf.Submit.Executed result -> 
                                 result |> function
                                           | Ok    profile -> { Head= RegistrationSucceeded profile; Tail=[] }
                                           | Error form    -> { Head= RegistrationFailed    form;    Tail=[] }

module Validation =
    open Nikeza.Mobile.Access.Commands.Registration.Validate

    type private RegistrationValidation = ResultOf.Validate -> RegistrationValidationEvent nonempty

    let events : RegistrationValidation = function
        ResultOf.Validate.Executed result -> 
                                   result |> function
                                             | Error form -> { Head= FormNotValidated form; Tail=[] }
                                             | Ok    form -> { Head= FormValidated    form; Tail=[] }