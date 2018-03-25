namespace Nikeza.Mobile.Profile

open Nikeza.Common
open Nikeza.DataTransfer

module Language =

    type Subscription = {
        Name     : string
        ImageUrl : string
    }

module Events =

    open Language

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

module Queries =

    open Language

    type PlatformsFn = unit-> Result<string list, string>
    type TopicsFn =    unit-> Result<Topic list, string>
    type SourcesFn =   unit-> Result<DataSourceRequest list, string>

    type SubscriptionsFn = ProfileId -> Result<Subscription list, ProfileId>