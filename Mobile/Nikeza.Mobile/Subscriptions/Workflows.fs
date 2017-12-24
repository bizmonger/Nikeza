module Workflows

open Commands
open Events

type Workflow = Command -> NotificationEvent list

let handle : Workflow =
    fun command -> command |> function
    | Subscribe   response -> response |> function
                                          | Ok    info       -> [SubscriberAdded     info.User]
                                          | Error profileId  -> [SubscriberAddFailed profileId]

    | Unsubscribe response -> response |> function
                                          | Ok    info       -> [SubscriberRemoved      info.User]
                                          | Error profileId  -> [SubscriberRemoveFailed profileId]