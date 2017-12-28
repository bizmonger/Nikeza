module internal Logic.Registration

open Events
open Registration
open Nikeza.Common
open Commands

type private Logic = ResultOf -> RegistrationEvent list

let handle : Logic =
    fun resultOf -> resultOf |> function
        | SubmitRegistration   result -> []
        | ValidateRegistration result -> []
        | _ -> []

let validate (unvalidatedForm:UnvalidatedForm) : Result<ValidatedForm, UnvalidatedForm> =

    let isValidEmail email = false

    let form = unvalidatedForm.Form

    if   not (form.Email |> isValidEmail) then
          Error unvalidatedForm

    elif form.Password <> form.Confirm then
          Error unvalidatedForm

    else  Ok { Form= form }

let isValid (credentials:LogInRequest) =
    let validEmail =    not <| System.String.IsNullOrEmpty(credentials.Email)
    let validPassword = not <| System.String.IsNullOrEmpty(credentials.Password)

    validEmail && validPassword