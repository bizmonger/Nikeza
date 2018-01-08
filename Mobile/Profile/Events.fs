module Nikeza.Mobile.Profile.Events

open Nikeza.Common
open Nikeza.DataTransfer

type UnvalidatedForm = Nikeza.Mobile.Profile.Registration.UnvalidatedForm
type ValidatedForm =   Nikeza.Mobile.Profile.Registration.ValidatedForm

type RegistrationValidationEvent =
    | FormValidated    of ValidatedForm
    | FormNotValidated of UnvalidatedForm

type RegistrationSubmissionEvent =
    | RegistrationSucceeded of Profile
    | RegistrationFailed    of ValidatedForm

type SessionEvent =
    | LoggedIn    of Provider
    | LoginFailed of Credentials
    | LoggedOut
    | LogoutFailed

type ProfileEvent =
    | ProfileRequested    of ProfileId

type SubscriptionEvent =
    | Subscribed          of ProviderId
    | Unsubscribed        of ProviderId

type ProfileEditorEvent =
    | ProfileValidated    of ValidatedProfile
    | ProfileNotValidated of EditedProfile
    | ProfileSaved        of Profile
    | ProfileSaveFailed   of ValidatedProfile

type RegistrationSubmissionEvent with
    member x.TryGetProfile() = 
           match x with
           | RegistrationSucceeded profile -> Some profile
           | _                             -> None

type ProfileEditorEvent with
    member x.TryGetProfile() =
           match x with
           | ProfileSaved profile -> Some profile
           | _                    -> None