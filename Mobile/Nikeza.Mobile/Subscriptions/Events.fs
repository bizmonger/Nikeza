module Nikeza.Mobile.Subscription.Events

open Nikeza.DataTransfer
open Nikeza.Common

type NotificationEvent =
    | LinksDiscovered        of Provider

    | SubscriberAdded        of User
    | SubscriberAddFailed    of ProfileId
    
    | SubscriberRemoved      of User
    | SubscriberRemoveFailed of ProfileId

type QueryEvent =
    | GetLatestSucceeded of Provider list
    | GetLatestFailed    of string