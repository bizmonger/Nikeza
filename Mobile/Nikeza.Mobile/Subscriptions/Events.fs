module Events

open Nikeza.DataTransfer
open Nikeza.Common

type NotificationEvent =
    | LinksDiscovered        of Provider

    | SubscriberAdded        of User
    | SubscriberAddFailed    of ProfileId
    
    | SubscriberRemoved      of User
    | SubscriberRemoveFailed of ProfileId