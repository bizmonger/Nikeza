namespace Nikeza.Mobile.Profile

open Nikeza.Mobile.Profile.Events
open Nikeza.Common

module Validate =
    open Nikeza.Mobile.Profile.Commands.ProfileEditor.Validate
    type private Handle = ResultOf.Editor -> ProfileValidateEvent nonempty

    let events : Handle = function
        ResultOf.Editor.Validate result -> 
                                 result |> function
                                           | Ok    profile -> { Head=ProfileValidated    profile; Tail=[] }
                                           | Error profile -> { Head=ProfileNotValidated profile; Tail=[] }

module Save =
    open Nikeza.Portal.Specification.Events
    open Nikeza
    open Nikeza.DataTransfer

    type private Handle = Result<DataTransfer.Profile, ValidatedProfile error> -> SaveProfileEvent nonempty

    let toEvents : Handle = function
        result -> 
        result |> function 
                  | Ok    profile -> { Head=SaveProfileSucceeded profile; Tail=[] }
                  | Error error   -> let profile = error.Context.Profile
                                     let error' = { Context=profile; Description=error.Description }
                                     { Head=SaveProfileFailed error'; Tail=[] }