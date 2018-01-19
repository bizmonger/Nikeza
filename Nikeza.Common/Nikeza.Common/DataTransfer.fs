module Nikeza.DataTransfer

open Nikeza.Common

[<CLIMutable>]
type Provider = ProviderRequest

[<CLIMutable>]
type User =     ProviderRequest

[<CLIMutable>]
type SubscriptionResponse = { User :  User; Provider: Provider }

[<CLIMutable>]
type ProfileAndTopics = ProfileAndTopicsRequest

[<CLIMutable>]
type Credentials = LogInRequest

[<CLIMutable>]
type Profile = ProfileRequest

type EditedProfile =    { Profile : Profile }
type ValidatedProfile = { Profile : Profile }

type LoginResponse = Provider

[<CLIMutable>]
type DataSourceSubmit = { 
    ProfileId: string
    Platform:  string
    AccessId:  string
}