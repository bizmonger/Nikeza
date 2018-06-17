module Nikeza.Mobile.Subscription.Workflow

open Nikeza.Mobile.Subscriptions.Command
open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Attempt
open Nikeza.Mobile.Subscriptions.Unsubscribe
    
type private FollowWorkflow =      FollowFn      -> Follow.Command      -> NotificationEvent list
type private UnsubscribeWorkflow = UnsubscribeFn -> Unsubscribe.Command -> NotificationEvent list

let follow : FollowWorkflow = 
    fun attempt -> function
        Follow.Command.Execute request -> 
                               request |> attempt
                                       |> toEvents
                                            
let unsubscribe : UnsubscribeWorkflow =
    fun attempt -> function
        Unsubscribe.Command.Execute request -> 
                                    request |> attempt
                                            |> toEvents