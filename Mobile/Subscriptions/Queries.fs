module Nikeza.Mobile.Subscriptions.Query

open Nikeza.Common

type RecentFn =        ProfileId -> Result<ProviderRequest list, ProfileId error>
type SubscriptionsFn = ProfileId -> Result<ProviderRequest list, ProfileId error>
type MembersFn =       unit      -> Result<ProviderRequest list, ProfileId error>

let recent : RecentFn =
    fun _ -> Error { Context= ProfileId "-1"; Description="Not impleented yet"}