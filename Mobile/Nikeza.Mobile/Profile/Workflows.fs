module Workflows

open Commands
open Events
open Registration
open Nikeza.Common

let private validate (unvalidatedForm:UnvalidatedForm) : RegistrationEvents list =

    let isValidEmail email = false

    let form = unvalidatedForm.Form

    if  not (form.Email |> isValidEmail) then
          [FormNotValidated unvalidatedForm]

    elif form.Password <> form.Confirm then
          [FormNotValidated unvalidatedForm]

    else  [FormValidated { Form= form }]

let isValid (credentials:LogInRequest) =
    let validEmail =    not <| System.String.IsNullOrEmpty(credentials.Email)
    let validPassword = not <| System.String.IsNullOrEmpty(credentials.Password)

    validEmail && validPassword

let handleRegistration = function
    | Validate form -> validate form
    | _ -> []

let handleSession = function
    | HandleLogin  response -> response |> function
                                           | Ok    provider    -> [LoggedIn    provider]
                                           | Error credentials -> [LoginFailed credentials]

    | HandleLogout response -> response |> function
                                           | Ok _              -> [LoggedOut]
                                           | Error credentials -> [LogOutFailed]
    | _ -> []

let handleUpdate = function
    | ValidateEdit profile  -> []
    | HandleSave   response -> response |> function
                                           | Ok    profile -> [ProfileSaved profile]
                                           | Error profile -> [ProfileSavedFailed profile]
    | _ -> []