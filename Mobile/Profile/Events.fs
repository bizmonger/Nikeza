module Nikeza.Mobile.Profile.Events

open Nikeza.Common
open Nikeza.DataTransfer

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