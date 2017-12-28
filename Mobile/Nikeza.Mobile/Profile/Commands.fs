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
    | SubmitRegistration   of Result<Nikeza.DataTransfer.Profile, Registration.ValidatedForm>
    | ValidateRegistration of Result<ValidatedForm, UnvalidatedForm>
                           
    | Login                of Result<Provider, Credentials>
    | Logout               of Result<unit, unit>
                           
    | ValidateEdit         of Result<ValidatedForm, UnvalidatedForm>
    | HandleSave           of Result<Profile, ProfileSubmitted>