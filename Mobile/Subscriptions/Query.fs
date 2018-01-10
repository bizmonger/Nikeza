module Nikeza.Mobile.Subscriptions.Query

open Nikeza.Common
open Nikeza.Mobile.Subscriptions.Events

type RecentFn =        ProfileId  -> Query
type SubscriptionsFn = ProfileId  -> Query
type MembersFn =       unit       -> Query

let latest : RecentFn =
    fun _ -> LatestFailed "Not yet implemented"