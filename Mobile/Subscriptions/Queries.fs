module Nikeza.Mobile.Subscriptions.Query

open Nikeza.Common

type GetRecent =        ProfileId -> Result<ProviderRequest list, ProfileId error>
type GetSubscriptions = ProfileId -> Result<ProviderRequest list, ProfileId error>
type GetMembers =       unit      -> Result<ProviderRequest list, ProfileId error>

let recent : GetRecent =
    fun _ -> Error { Context= ProfileId "-1"; Description="Not impleented yet"}