module Events

open Nikeza.Common
open Nikeza.Mobile.Registration

type RegistrationEvents =
    | FormValidated         of Registration.UnvalidatedForm
    | FormNotValidated      of Registration.UnvalidatedForm
    | FormSubmitted         of Registration.ValidatedForm
    | RegistrationSucceeded of Profile
    | RegistrationFailed    of Registration.ValidatedForm
    | LoginRequested        of ProfileId

type AuthenticationEvents =
    | LoggedIn  of Profile
    | LoggedOut of Profile

type ProfileEvents =
    | ProfileRequested of ProfileId
    | ProfileSaved     of Registration.Validated
    | Subscribed       of ProviderId
    | Unsubscribed     of ProviderId