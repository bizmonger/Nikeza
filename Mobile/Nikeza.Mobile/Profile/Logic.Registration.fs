module internal Are.Registration

open Nikeza.Mobile.Profile.Commands
open Nikeza.Mobile.Profile.Events

module Submission =
    type private RegistrationSubmission = ResultOf.Submit -> RegistrationSubmissionEvent list

    let events : RegistrationSubmission =
        fun resultOf -> 
            resultOf |> function
                        ResultOf.Submit.Executed result -> 
                                                 result |> function
                                                         | Ok    profile -> [ RegistrationSucceeded profile]
                                                         | Error form    -> [ RegistrationFailed    form ]

module Validation =
    type private RegistrationValidation = ResultOf.Validation -> RegistrationValidationEvent list

    let events : RegistrationValidation =
        fun resultOf -> 
            resultOf |> function
                        ResultOf.Validation.BeingExecuted result -> 
                                                          result |> function
                                                                    | Error form -> [FormNotValidated form]
                                                                    | Ok    form -> [FormValidated    form]