module Workflows

open Commands
open Events

let handleSubscription = function
    | Subscribe   response -> response |> function
                                          | Ok    info       -> [SubscriberAdded     info.User]
                                          | Error profileId  -> [SubscriberAddFailed profileId]

    | Unsubscribe response -> response |> function
                                          | Ok    info       -> [SubscriberRemoved    info.User]
                                          | Error profileId  -> [SubscriberRemoveFailed profileId]