module internal Logic.Registration

open System
open Nikeza.Common
open Nikeza.Access.Specification

type ValidatedForm =   Nikeza.Access.Specification.ValidatedForm
type UnvalidatedForm = Nikeza.Access.Specification.UnvalidatedForm

let validate (unvalidatedForm:UnvalidatedForm) : Result<ValidatedForm, UnvalidatedForm> =

    let  isValidEmail (Email email) = email |> String.length > 3

    let  form = unvalidatedForm.Form

    let (password,confirm) =
        (form.Password |> function Password p -> p, form.Confirm  |> function Password p -> p)

    if   not (form.Email |> isValidEmail) then
         Error unvalidatedForm

    elif String.length password < 3 then
         Error unvalidatedForm

    elif not (password = confirm) then
         Error unvalidatedForm

    else Ok { Form= form }

let isValid (credentials:LogInRequest) =
    let validEmail =    not <| String.IsNullOrEmpty(credentials.Email)
    let validPassword = not <| String.IsNullOrEmpty(credentials.Password)

    validEmail && validPassword