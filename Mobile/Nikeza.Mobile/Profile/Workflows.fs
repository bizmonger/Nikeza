module Workflows

open Commands
open Events
open Registration
open Nikeza.Common
open Nikeza.DataTransfer

let private validateRegistration (unvalidatedForm:UnvalidatedForm) : RegistrationEvents list =

    let isValidEmail email = false

    let form = unvalidatedForm.Form

    if  not (form.Email |> isValidEmail) then
          [FormNotValidated unvalidatedForm]

    elif form.Password <> form.Confirm then
          [FormNotValidated unvalidatedForm]

    else  [FormValidated { Form= form }]

let private isValid (credentials:LogInRequest) =
    let validEmail =    not <| System.String.IsNullOrEmpty(credentials.Email)
    let validPassword = not <| System.String.IsNullOrEmpty(credentials.Password)

    validEmail && validPassword

let private validateEdit (edited:ProfileEdited) =
    let validEmail =     not <| System.String.IsNullOrEmpty(edited.Profile.Email)
    let validFirstName = not <| System.String.IsNullOrEmpty(edited.Profile.FirstName)
    let validLastName =  not <| System.String.IsNullOrEmpty(edited.Profile.LastName)

    if validEmail && validFirstName && validLastName
       then [ProfileValidated    edited]
       else [ProfileNotValidated edited]

let handleRegistration = function
    | Validate form -> validateRegistration form
    | _ -> []

let handleSession = function
    | HandleLogin  response -> response |> function
                                           | Ok    provider    -> [LoggedIn    provider]
                                           | Error credentials -> [LoginFailed credentials]

    | HandleLogout response -> response |> function
                                           | Ok _              -> [LoggedOut]
                                           | Error credentials -> [LogoutFailed]
    | _ -> []

let handleUpdate = function
    | ValidateEdit profile  -> validateEdit profile 
    | HandleSave   response -> response |> function
                                           | Ok    profile -> [ProfileSaved      profile]
                                           | Error profile -> [ProfileSaveFailed profile]
    | _ -> []