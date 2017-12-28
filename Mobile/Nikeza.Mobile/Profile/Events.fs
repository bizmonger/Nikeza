module Events

open Nikeza.Common
open Nikeza.DataTransfer

type RegistrationEvent =
    | FormValidated         of Registration.UnvalidatedForm
    | FormNotValidated      of Registration.UnvalidatedForm
    | FormSubmitted         of Registration.ValidatedForm
    | RegistrationSucceeded of Profile
    | RegistrationFailed    of Registration.ValidatedForm
    | LoginRequested        of ProfileId

type SessionEvent =
    | LoggedIn    of Provider
    | LoginFailed of Credentials
    | LoggedOut
    | LogoutFailed

type ProfileEvent =
    | ProfileValidated    of ValidatedProfile
    | ProfileNotValidated of EditedProfile
    | ProfileRequested    of ProfileId
    | ProfileSaved        of Nikeza.DataTransfer.Profile
    | ProfileSaveFailed   of ValidatedProfile
    | Subscribed          of ProviderId
    | Unsubscribed        of ProviderId