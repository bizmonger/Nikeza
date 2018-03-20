module Nikeza.Mobile.Access.Events

open Nikeza.DataTransfer

type UnvalidatedForm = Nikeza.Mobile.Access.UnvalidatedForm
type ValidatedForm =   Nikeza.Mobile.Access.ValidatedForm

type RegistrationValidationEvent =
    | FormValidated    of ValidatedForm
    | FormNotValidated of UnvalidatedForm

type RegistrationSubmissionEvent =
    | RegistrationSucceeded of Profile
    | RegistrationFailed    of ValidatedForm

type LoginEvent =
    | LoggedIn    of Provider
    | LoginFailed of Credentials

type LogoutEvent =
    | LoggedOut    of Provider
    | LogoutFailed of Provider