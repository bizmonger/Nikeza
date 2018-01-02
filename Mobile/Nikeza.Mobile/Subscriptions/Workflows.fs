module Execute

open Nikeza.Mobile.Subscription.Commands
open Nikeza.Mobile.Subscription.Events
open Nikeza.Mobile.Subscriptions.Try

module Subscriptions =
    open Nikeza.Mobile.Subscriptions

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