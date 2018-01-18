module Nikeza.Mobile.Subscriptions.Events

open Nikeza.DataTransfer
open Nikeza.Common

type NotificationEvent =
    | LinksDiscovered        of Provider

    | SubscriberAdded        of ProfileId
    | SubscriberAddFailed    of ProfileId
    
    | SubscriberRemoved      of ProfileId
    | SubscriberRemoveFailed of ProfileId

type RecentQuery =
    | RecentSucceeded of Provider list
    | RecentFailed    of string

type MembersQuery =
    | MembersSucceeded of Provider list
    | MembersFailed    of string

type SubscriptionsQuery =
    | SubscriptionsSucceeded of Provider list
    | SubscriptionsFailed    of string