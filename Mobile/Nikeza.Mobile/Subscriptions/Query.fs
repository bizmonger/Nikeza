module Nikeza.Mobile.Subscriptions.Query

open Nikeza.Mobile.Subscription.Events

open Nikeza.Common

type RecentFn = ProfileId -> QueryEvent list

let latest : RecentFn =
    fun _ -> [GetLatestFailed "Not yet implemented"]