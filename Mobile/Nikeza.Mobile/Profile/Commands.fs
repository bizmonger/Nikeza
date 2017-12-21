module Commands

open Nikeza.Mobile.Registration
open Nikeza.Common

type Command =
    | Validate      of Registration.UnvalidatedForm
    | TrySubmit     of Registration.ValidatedForm

    | Login         of LogInRequest

    | Edit          of Profile
    | Save          of Profile