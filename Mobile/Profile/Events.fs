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

type SubscriptionEvent =
    | Subscribed   of ProviderId
    | Unsubscribed of ProviderId

type ProfileValidateEvent =
    | ProfileValidated    of ValidatedProfile
    | ProfileNotValidated of EditedProfile

type ProfileSaveEvent =
    | ProfileSaved      of Profile
    | ProfileSaveFailed of ValidatedProfile

type SourcesSaveEvent =
    | SourcesSaved  of Profile
    | SourcesFailed of DataSourceSubmit list

type QueryTopicsFailed = QueryTopicsFailed of string