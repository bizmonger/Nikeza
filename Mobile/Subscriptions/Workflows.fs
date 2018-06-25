module Nikeza.Mobile.Subscription.Workflow

open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Attempt
open Nikeza.Mobile.Subscriptions
open Nikeza.Common
    
type private FollowWorkflow =      FollowFn      -> FollowRequest      -> NotificationEvent list
type private UnsubscribeWorkflow = UnsubscribeFn -> UnsubscribeRequest -> NotificationEvent list

let follow : FollowWorkflow = 
    fun attempt request ->
                request |> attempt
                        |> FollowResponse.toEvents
                                            
let unsubscribe : UnsubscribeWorkflow =
    fun attempt request ->
                request |> attempt
                        |> UnsubscribeResponse.toEvents