namespace Nikeza.Access.Specification

open Nikeza
open DataTransfer

module Events =

    type RegistrationValidationEvent =
        | FormValidated    of ValidatedForm
        | FormNotValidated of UnvalidatedForm

    type RegistrationSubmissionEvent =
        | RegistrationSucceeded of DataTransfer.Profile
        | RegistrationFailed    of ValidatedForm

    type LoginEvent =
        | LoggedIn             of Provider
        | FailedToConnect      of Credentials
        | FailedToAuthenticate of Credentials

    type LogoutEvent =
        | LoggedOut    of Provider
        | LogoutFailed of Provider