module internal Logic.Registration

open System
open Nikeza.Common
open Nikeza.Access.Specification
open Nikeza.Access.Specification.Validation

type ValidatedForm =   Nikeza.Access.Specification.ValidatedForm
type UnvalidatedForm = Nikeza.Access.Specification.UnvalidatedForm

let validate : ValidateForm =

    fun (Unvalidated form) ->

        let  isValidEmail (Email email) = email |> String.length > 3

        let (password,confirm) =
            (form.Password |> function Password p -> p, form.Confirm  |> function Password p -> p)

        if   not (form.Email |> isValidEmail) then
             Unvalidated form |> Error

        elif String.length password < 3 then
             Error <| Unvalidated form

        elif not (password = confirm) then
             Error <| Unvalidated form

        else Ok <| Validated form

let isValid (credentials:LogInRequest) =
    let validEmail =    not <| String.IsNullOrEmpty(credentials.Email)
    let validPassword = not <| String.IsNullOrEmpty(credentials.Password)

    validEmail && validPassword