module Nikeza.Mobile.Subscriptions.Events

open Nikeza.DataTransfer
open Nikeza.Common

type NotificationEvent =
    | LinksDiscovered        of Provider

    | SubscriberAdded        of ProfileId
    | SubscriberAddFailed    of ProfileId
    
    | SubscriberRemoved      of ProfileId
    | SubscriberRemoveFailed of ProfileId

type Query =
    | RecentSucceeded  of Provider list
    | LatestFailed     of string
                              
    | MembersSucceeded of Provider list
    | MembersFailed    of string

    | SubscriptionsSucceeded of Provider list
    | SubscriptionsFailed    of string