module internal Are.Editor

open Nikeza.Mobile.Profile.Events

module Validate =
    open Nikeza.Mobile.Profile.Commands.ProfileEditor.Validate
    type private Handle = ResultOf.Editor -> ProfileValidateEvent list

    let events : Handle = function
        ResultOf.Editor.Validate result -> 
                                 result |> function
                                           | Ok    profile -> [ProfileValidated    profile]
                                           | Error profile -> [ProfileNotValidated profile]

module Save =
    open Nikeza.Mobile.Profile.Commands.ProfileEditor.Save

    type private Handle = ResultOf.Editor -> ProfileSaveEvent list

    let events : Handle = function
        ResultOf.Editor.Save result -> 
                             result |> function
                                       | Ok    profile -> [ProfileSaved      profile]
                                       | Error profile -> [ProfileSaveFailed profile]