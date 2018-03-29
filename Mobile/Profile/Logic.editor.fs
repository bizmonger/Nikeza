namespace Are.Editor

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
    open Nikeza.Mobile.Profile.Commands.ProfileEditor.Save

    type private Handle = ResultOf.Editor -> SaveProfileEvent nonempty

    let events : Handle = function
        ResultOf.Editor.Save result -> { Head=result; Tail=[] }