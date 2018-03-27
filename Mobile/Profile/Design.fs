namespace Nikeza.Mobile.Profile

open Nikeza.Common
open Nikeza.DataTransfer

module Events =

    type SaveProfileEvent =     Result<Profile,ValidatedProfile>
    type SaveDataSourcesEvent = Result<Profile, DataSourceSubmit list>

    type SubscriptionEvent =
        | Subscribed   of ProviderId
        | Unsubscribed of ProviderId

    type ProfileValidateEvent =
        | ProfileValidated    of ValidatedProfile
        | ProfileNotValidated of EditedProfile

module Queries =

    type PlatformsFn = unit-> Result<string list, string>
    type TopicsFn =    unit-> Result<Topic list, string>
    type SourcesFn =   unit-> Result<DataSourceRequest list, string>