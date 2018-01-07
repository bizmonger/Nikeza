﻿module Nikeza.Mobile.Profile.Events

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
    | ProfileValidated    of ValidatedProfile
    | ProfileNotValidated of EditedProfile
    | ProfileRequested    of ProfileId
    | ProfileSaved        of Profile
    | ProfileSaveFailed   of ValidatedProfile
    | Subscribed          of ProviderId
    | Unsubscribed        of ProviderId

type RegistrationSubmissionEvent with
    member x.TryGetProfile() = 
           match x with
           | RegistrationSucceeded profile -> Some profile
           | _                             -> None
    