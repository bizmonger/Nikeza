module internal Edit

open Nikeza.DataTransfer

type private Validate = EditedProfile -> Result<ValidatedProfile, EditedProfile>

let validate : Validate =
    fun edit ->
        let validEmail =     not <| System.String.IsNullOrEmpty(edit.Profile.Email)
        let validFirstName = not <| System.String.IsNullOrEmpty(edit.Profile.FirstName)
        let validLastName =  not <| System.String.IsNullOrEmpty(edit.Profile.LastName)

        if validEmail && validFirstName && validLastName
            then Ok    { Profile= edit.Profile }
            else Error { Profile= edit.Profile }