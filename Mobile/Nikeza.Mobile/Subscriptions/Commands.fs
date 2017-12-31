module Nikeza.Mobile.Subscription.Commands

open Nikeza.Common

type Command =
    | Follow      of FollowRequest
    | Unsubscribe of UnsubscribeRequest