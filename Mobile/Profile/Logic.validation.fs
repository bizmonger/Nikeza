module internal Logic.Registration

open Nikeza.Common
open Nikeza.Mobile.Profile.Registration

type ValidatedForm =   Nikeza.Mobile.Profile.Registration.ValidatedForm
type UnvalidatedForm = Nikeza.Mobile.Profile.Registration.UnvalidatedForm

let validate (unvalidatedForm:UnvalidatedForm) : Result<ValidatedForm, UnvalidatedForm> =

    let  isValidEmail (Email email) = email |> String.length > 3

    let  form = unvalidatedForm.Form

    if   not (form.Email |> isValidEmail) then
         Error unvalidatedForm

    elif form.Password <> form.Confirm then
         Error unvalidatedForm

    else Ok { Form= form }

let isValid (credentials:LogInRequest) =
    let validEmail =    not <| System.String.IsNullOrEmpty(credentials.Email)
    let validPassword = not <| System.String.IsNullOrEmpty(credentials.Password)

    validEmail && validPassword