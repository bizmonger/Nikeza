module internal Logic.Edit

open Nikeza.DataTransfer
open Events
open Commands

type private Validate = EditedProfile -> Result<ValidatedProfile, EditedProfile>
type private Handle =   ResultOf -> ProfileEvent list

let validate : Validate =
    fun edit ->
        let validEmail =     not <| System.String.IsNullOrEmpty(edit.Profile.Email)
        let validFirstName = not <| System.String.IsNullOrEmpty(edit.Profile.FirstName)
        let validLastName =  not <| System.String.IsNullOrEmpty(edit.Profile.LastName)

        if validEmail && validFirstName && validLastName
           then Ok    { Profile= edit.Profile }
           else Error { Profile= edit.Profile }

let handle : Handle =
    fun response -> response |> function
                                | ResultOf.ValidateProfile result -> result |> function
                                                                               | Ok    profile -> [ProfileValidated    profile]
                                                                               | Error profile -> [ProfileNotValidated profile]
                                | ResultOf.Save            result -> result |> function
                                                                               | Ok    profile -> [ProfileSaved      profile]
                                                                               | Error profile -> [ProfileSaveFailed profile]
                                | _ -> []


