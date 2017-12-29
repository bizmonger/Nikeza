module internal Logic.Registration

open Nikeza.Common
open Events
open Registration
open Commands

type private Registration = ResultOf.Registration -> RegistrationEvent list

let handle : Registration =
    fun resultOf -> resultOf |> function
        | ResultOf.Registration.Submit   result -> result |> function
                                                             | Ok    profile -> [RegistrationSucceeded profile]
                                                             | Error form    -> [RegistrationFailed    form]

        | ResultOf.Registration.Validate result -> result |> function
                                                             | Ok    form -> [FormValidated    form]
                                                             | Error form -> [FormNotValidated form]

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