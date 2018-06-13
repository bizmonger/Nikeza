namespace Nikeza.Portal.Specification

open Nikeza.Common

module Query =
   
    type RecentFn =        ProfileId -> Result<ProviderRequest list, ProfileId error>
    type SubscriptionsFn = ProfileId -> Result<ProviderRequest list, ProfileId error>
    type MembersFn =       unit      -> Result<ProviderRequest list, ProfileId error>