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

    type QueryTopicsEvent =
        | QueryTopicsSucceeded of Topic list
        | QueryTopicsFailed    of string

    type QuerySourcesEvent =
        | QuerySourcesSucceeded of DataSourceRequest list
        | QuerySourcesFailed    of string

    type QueryPlatformsEvent =
        | QueryPlatformsSucceeeded of string list
        | QueryPlatformsFailed     of string

    type QuerySubscriptionsEvent =
        | QuerySubscriptionsSucceeeded of Subscription list
        | QuerySubscriptionsFailed     of ProfileId

module Queries =

    open Events

    type PlatformsFn = unit-> QueryPlatformsEvent
    type TopicsFn =    unit-> QueryTopicsEvent   
    type SourcesFn =   unit-> QuerySourcesEvent  

    type SubscriptionsFn = ProfileId -> QuerySubscriptionsEvent