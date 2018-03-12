module Nikeza.Mobile.Subscription.Workflow

open Nikeza.Mobile.Subscriptions.Command
open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Try
open Nikeza.Mobile.Subscriptions.Try.Follow
open Nikeza.Mobile.Subscriptions.Try.Unsubscribe
    
type private FollowWorkflow =      FollowFn      -> Follow.Command      -> NotificationEvent list
type private UnsubscribeWorkflow = UnsubscribeFn -> Unsubscribe.Command -> NotificationEvent list

let follow : FollowWorkflow = 
    fun followFn -> function
        Follow.Command.Execute request -> 
                               request |> followFn
                                       |> ResultOf.Follow 
                                       |> Are.Subscription.Follow.events
                                            
let unsubscribe : UnsubscribeWorkflow =
    fun unsubscribeFn -> function
        Unsubscribe.Command.Execute request -> 
                                    request |> unsubscribeFn
                                            |> ResultOf.Unsubscribe  
                                            |> Are.Subscription.Unsubscribe.events