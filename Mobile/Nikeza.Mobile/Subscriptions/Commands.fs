module Nikeza.Mobile.Subscriptions.Command

open Nikeza.Common

module Follow =
    type Command = Execute of FollowRequest

module Unsubscribe =
    type Command = Execute of UnsubscribeRequest