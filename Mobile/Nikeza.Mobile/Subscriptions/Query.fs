module Nikeza.Mobile.Subscriptions.Query

open Nikeza.Common
open Nikeza.Mobile.Subscriptions.Events

type RecentFn =        ProfileId -> QueryEvent list
type SubscriptionsFn = ProfileId -> QueryEvent list
type MembersFn =       unit      -> QueryEvent list

let latest : RecentFn =
    fun _ -> [GetLatestFailed "Not yet implemented"]