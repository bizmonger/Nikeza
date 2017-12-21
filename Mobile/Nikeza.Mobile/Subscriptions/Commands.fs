module Nikeza.Mobile.Subscriptions.Commands

open Nikeza.Common

type Command =
    | Subscribe     of ProviderId
    | Unsubscribe   of ProviderId