module Nikeza.Mobile.Subscriptions.Events

open Nikeza.DataTransfer
open Nikeza.Common

type NotificationEvent =
    | LinksDiscovered        of Provider

    | SubscriberAdded        of ProfileId
    | SubscriberAddFailed    of ProfileId
    
    | SubscriberRemoved      of ProfileId
    | SubscriberRemoveFailed of ProfileId

type GetProfilesEvent =
    | GetRecentFailed        of ProfileId error
    | GetMembersFailed       of ProfileId error
    | GetSubscriptionsFailed of ProfileId error