module internal WorkflowDetails

open Events
open Registration
open Nikeza.Common
open Nikeza.DataTransfer

let validateRegistration (unvalidatedForm:UnvalidatedForm) : RegistrationEvent list =

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

let validateEdit (edited:ProfileEdited) =
    let validEmail =     not <| System.String.IsNullOrEmpty(edited.Profile.Email)
    let validFirstName = not <| System.String.IsNullOrEmpty(edited.Profile.FirstName)
    let validLastName =  not <| System.String.IsNullOrEmpty(edited.Profile.LastName)

    if validEmail && validFirstName && validLastName
       then [ProfileValidated    edited]
       else [ProfileNotValidated edited]