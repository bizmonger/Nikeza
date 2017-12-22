module Nikeza.DataTransfer

open Nikeza.Common

[<CLIMutable>]
type Provider = ProviderRequest

[<CLIMutable>]
type ProfileAndTopics = ProfileAndTopicsRequest

[<CLIMutable>]
type Credentials = LogInRequest

[<CLIMutable>]
type Profile = ProfileRequest

type ProfileDraft =     { Profile : Profile }

[<CLIMutable>]
type ProfileSubmitted = { Profile : Profile }

