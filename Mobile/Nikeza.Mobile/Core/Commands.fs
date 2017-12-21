module Command

open Model
open Nikeza.Common

type Command =
    | FeatureLink   of Id
    | UnfeatureLink of Id

    | Validate      of Registration.UnvalidatedForm
    | TrySubmit     of Registration.ValidatedForm

    | Login         of Credentials
    | Edit          of Profile
    | View          of Provider
    | Save          of Profile

    | Subscribe     of Provider
    | Unsubscribe   of Provider

    | Request       of Page