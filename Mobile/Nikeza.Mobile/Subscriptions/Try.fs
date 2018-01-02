module Nikeza.Mobile.Subscriptions.Try

open Nikeza.Common
open Nikeza.DataTransfer

type Request<'request, 'response, 'errorInfo> = 
             'request -> Result<'response, 'errorInfo>

let follow :       Request<FollowRequest, SubscriptionResponse, ProfileId> =
    fun request ->
        Error (request.ProfileId |> ProfileId)

let unsubscribe :  Request<UnsubscribeRequest, SubscriptionResponse, ProfileId> =
    fun request ->
        Error (request.ProfileId |> ProfileId)

type ResultOf =
    | Follow      of Result<SubscriptionResponse, ProfileId>
    | Unsubscribe of Result<SubscriptionResponse, ProfileId>