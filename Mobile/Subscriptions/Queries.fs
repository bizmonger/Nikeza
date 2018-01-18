module Nikeza.Mobile.Subscriptions.Query

open Nikeza.Common
open Nikeza.Mobile.Subscriptions.Events

type RecentFn =        ProfileId -> RecentQuery
type SubscriptionsFn = ProfileId -> SubscriptionsQuery
type MembersFn =       unit      -> MembersQuery

let recent : RecentFn =
    fun _ -> RecentFailed "Not yet implemented"