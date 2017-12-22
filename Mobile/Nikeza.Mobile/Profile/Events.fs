module Events

open Nikeza.Common
open Nikeza.DataTransfer

type RegistrationEvents =
    | FormValidated         of Registration.UnvalidatedForm
    | FormNotValidated      of Registration.UnvalidatedForm
    | FormSubmitted         of Registration.ValidatedForm
    | RegistrationSucceeded of Profile
    | RegistrationFailed    of Registration.ValidatedForm
    | LoginRequested        of ProfileId

type AuthenticationEvents =
    | LoggedIn     of Provider
    | LoginFailed  of LogInRequest
    | LogOutFailed
    | LoggedOut

type ProfileEvents =
    | ProfileRequested   of ProfileId
    | ProfileSaved       of Nikeza.DataTransfer.Profile
    | ProfileSavedFailed of ProfileSubmitted
    | Subscribed         of ProviderId
    | Unsubscribed       of ProviderId