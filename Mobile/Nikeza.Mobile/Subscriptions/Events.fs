module Events

open Nikeza.DataTransfer
open Nikeza.Common

type NotificationEvents =
    | LinksDiscovered        of Provider

    | SubscriberAdded        of User
    | SubscriberAddFailed    of ProfileId
    
    | SubscriberRemoved      of User
    | SubscriberRemoveFailed of ProfileId