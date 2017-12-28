module Commands

open Nikeza.DataTransfer
open Registration

type Command =
    | ValidateRegistration of Registration.UnvalidatedForm
    | SubmitRegistration   of Registration.ValidatedForm
                           
    | Login                of Credentials
    | Logout               
                                 
    | ValidateEdit         of EditedProfile
    | Save                 of ValidatedProfile
                           
type ResultOf =            
    | SubmitRegistration   of Result<Nikeza.DataTransfer.Profile, Registration.ValidatedForm>
    | ValidateRegistration of Result<ValidatedForm, UnvalidatedForm>
                           
    | Login                of Result<Provider, Credentials>
    | Logout               of Result<unit, unit>
                           
    | ValidateProfile      of Result<ValidatedProfile, EditedProfile>
    | Save                 of Result<Profile, ValidatedProfile>