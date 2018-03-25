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

    type QueryTopicsFailed = QueryTopicsFailed of string

    type QuerySubscriptionsEvent =
        | QuerySucceeeded of Subscription list
        | QueryFailed     of ProfileId

module Queries =

    open Events

    type SubscriptionsFn = ProfileId -> QuerySubscriptionsEvent list