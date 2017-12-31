module internal Are.Edit

open Nikeza.Mobile.Profile.Events
open Commands

type private Handle =   ResultOf.Editor -> ProfileEvent list

let events : Handle =
    fun response -> 
        response |> function
                    | ResultOf.Editor.Validate result -> 
                                                result |> function
                                                            | Ok    profile -> [ProfileValidated    profile]
                                                            | Error profile -> [ProfileNotValidated profile]
                    | ResultOf.Editor.Save     result -> 
                                                result |> function
                                                            | Ok    profile -> [ProfileSaved      profile]
                                                            | Error profile -> [ProfileSaveFailed profile]