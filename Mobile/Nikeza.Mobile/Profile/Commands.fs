module Commands

open Nikeza.DataTransfer
open Registration

type Command =
    | ValidateRegistration of Registration.UnvalidatedForm
    | SubmitRegistration   of Registration.ValidatedForm
                           
    | HandleLogin          of Credentials
    | HandleLogout         
                           
    | ValidateEdit         of ProfileEdited
    | HandleSave           of ProfileEdited
                           
type ResultOf =            
    | SubmitRegistration   of Result<Profile, Registration.ValidatedForm>
    | ValidateRegistration of Result<ValidatedForm, UnvalidatedForm>
                           
    | HandleLogin          of Result<Provider, Credentials>
    | HandleLogout         of Result<unit, unit>
                           
    | ValidateEdit         of Result<ProfileEdited, ProfileEdited>
    | HandleSave           of Result<Profile, ProfileSubmitted>