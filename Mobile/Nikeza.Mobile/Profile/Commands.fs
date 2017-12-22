module Commands

open Nikeza.DataTransfer

type Command =
    | ValidateRegistration of Registration.UnvalidatedForm

    | HandleLogin          of Result<Provider, Credentials>
    | HandleLogout         of Result<unit, unit>

    | ValidateEdit         of ProfileEdited
    | HandleSave           of Result<Profile, ProfileSubmitted>