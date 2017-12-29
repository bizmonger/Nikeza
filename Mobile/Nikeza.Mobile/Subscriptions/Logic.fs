module internal Logic

open Events
open Try

type private Logic = ResultOf -> NotificationEvent list

module Result =

    let handle : Logic =
        fun command -> command |> function
        | Follow      response -> 
                      response |> function
                                  | Ok    info       -> [SubscriberAdded        info.User]
                                  | Error profileId  -> [SubscriberAddFailed    profileId]
        | Unsubscribe response -> 
                      response |> function
                                  | Ok    info       -> [SubscriberRemoved      info.User]
                                  | Error profileId  -> [SubscriberRemoveFailed profileId]