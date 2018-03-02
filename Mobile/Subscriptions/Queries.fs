module Nikeza.Mobile.Subscriptions.Query

open Nikeza.Common

type RecentFn =        ProfileId -> Result<ProviderRequest list, string>
type SubscriptionsFn = ProfileId -> Result<ProviderRequest list, string>
type MembersFn =       unit      -> Result<ProviderRequest list, string>

let recent : RecentFn =
    fun _ -> Error "Not yet implemented"