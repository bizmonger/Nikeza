namespace Nikeza.Mobile.Profile

open Nikeza.Common
open Nikeza.DataTransfer

module Validate =

    open Nikeza.Mobile.Profile.Events

    type private Handle = Result<ValidatedProfile, EditedProfile> -> ProfileValidateEvent nonempty

    let toEvents : Handle = function
        result -> 
        result |> function
                  | Ok    profile -> { Head=ProfileValidated    profile; Tail=[] }
                  | Error profile -> { Head=ProfileNotValidated profile; Tail=[] }

module Save =
    open Nikeza.Portal.Specification.Events
    open Nikeza

    type private Handle = Result<DataTransfer.Profile, ValidatedProfile error> -> SaveProfileEvent nonempty

    let toEvents : Handle = function
        result -> 
        result |> function 
                  | Ok    profile -> { Head=SaveProfileSucceeded profile; Tail=[] }
                  | Error error   -> let profile = error.Context.Profile
                                     let error' = { Context=profile; Description=error.Description }
                                     { Head=SaveProfileFailed error'; Tail=[] }