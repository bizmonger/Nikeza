module Execute

open Commands
open Try
open Events

module Subscriptions =
    type private Workflow = Command -> NotificationEvent list

    let workflow : Workflow = 
        fun command -> command |> function
        | Command.Follow      request -> 
                              request |> Try.follow 
                                      |> ResultOf.Follow 
                                      |> Are.Subscription.events
        | Command.Unsubscribe request -> 
                              request |> Try.unsubscribe 
                                      |> ResultOf.Unsubscribe  
                                      |> Are.Subscription.events