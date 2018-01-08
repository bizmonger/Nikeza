module internal Are.Edit

open Nikeza.Mobile.Profile.Events

module Validate =
    open Nikeza.Mobile.Profile.Commands.ProfileEditor.Validate
    type private Handle = ResultOf.Editor -> ProfileEditorEvent list

    let events : Handle =
        fun response -> 
            response |> function
                        | ResultOf.Editor.Validate result -> 
                                                   result |> function
                                                             | Ok    profile -> [ProfileValidated    profile]
                                                             | Error profile -> [ProfileNotValidated profile]

module Save =
    open Nikeza.Mobile.Profile.Commands.ProfileEditor.Save

    type private Handle = ResultOf.Editor -> ProfileEditorEvent list

    let events : Handle =
        fun response -> 
            response |> function
                        | ResultOf.Editor.Save     result -> 
                                                   result |> function
                                                             | Ok    profile -> [ProfileSaved      profile]
                                                             | Error profile -> [ProfileSaveFailed profile]