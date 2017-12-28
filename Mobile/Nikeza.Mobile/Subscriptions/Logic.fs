module internal Logic

open IO
open Events

type private Logic = ResultOf -> NotificationEvent list

let handle : Logic =
    fun command -> command |> function
    | Follow      response -> response |> function
                                          | Ok    info       -> [SubscriberAdded        info.User]
                                          | Error profileId  -> [SubscriberAddFailed    profileId]

    | Unsubscribe response -> response |> function
                                          | Ok    info       -> [SubscriberRemoved      info.User]
                                          | Error profileId  -> [SubscriberRemoveFailed profileId]