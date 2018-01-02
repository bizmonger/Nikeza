module Nikeza.Mobile.Subscriptions.Query

open Nikeza.Mobile.Subscription.Events

open Nikeza.Common

type LatestFn = ProfileId -> QueryEvent list

let latest : LatestFn =
    fun _ -> 
        [GetLatestFailed "Not yet implemented"]