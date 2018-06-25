module Nikeza.Mobile.Subscriptions.Attempt

open Nikeza.Common
open Nikeza.DataTransfer

type Request<'request, 'response, 'errorInfo> = 
             'request -> Result<'response, 'errorInfo>

type FollowFn =      FollowRequest      -> Result<SubscriptionResponse, ProfileId>
type UnsubscribeFn = UnsubscribeRequest -> Result<SubscriptionResponse, ProfileId>

let follow : FollowFn =
    fun request ->
        Error <| ProfileId request.ProfileId

let unsubscribe :  UnsubscribeFn =
    fun request ->
        Error <| ProfileId request.ProfileId