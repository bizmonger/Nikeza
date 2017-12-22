module Commands

open Nikeza.Common
open Nikeza.DataTransfer

type Command =
    | Validate     of Registration.UnvalidatedForm

    | HandleLogin  of Result<Provider, Credentials>
    | HandleLogout of Result<unit,     Credentials>

    | ValidateEdit of ProfileEdited
    | HandleSave   of Result<Profile, ProfileSubmitted>