module internal Are.Registration

open Nikeza.Mobile.Access.Events

module Submission =
    open Nikeza.Mobile.Access.Commands.Registration.Submit

    type private RegistrationSubmission = ResultOf.Submit -> RegistrationSubmissionEvent list

    let events : RegistrationSubmission = function
        ResultOf.Submit.Executed result -> 
                                 result |> function
                                           | Ok    profile -> [ RegistrationSucceeded profile]
                                           | Error form    -> [ RegistrationFailed    form ]

module Validation =
    open Nikeza.Mobile.Access.Commands.Registration.Validate

    type private RegistrationValidation = ResultOf.Validate -> RegistrationValidationEvent list

    let events : RegistrationValidation = function
        ResultOf.Validate.Executed result -> 
                                   result |> function
                                             | Error form -> [FormNotValidated form]
                                             | Ok    form -> [FormValidated    form]