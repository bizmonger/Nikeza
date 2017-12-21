module Subscriptions

open Nikeza.Common

type NotificationEvents =
    | ContentDiscovered of ProviderRequest
    | NewSubscriber     of Profile