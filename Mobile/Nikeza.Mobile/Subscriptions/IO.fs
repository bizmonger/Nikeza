module IO

open Nikeza.Common
open Nikeza.DataTransfer

type TryRequest<'request, 'response, 'errorInfo> = 
                'request -> Result<'response, 'errorInfo>

let tryFollow :       TryRequest<FollowRequest, SubscriptionResponse, ProfileId> =
    fun (request:FollowRequest) ->
        Error (request.ProfileId |> ProfileId)

let tryUnsubscribe :  TryRequest<UnsubscribeRequest, SubscriptionResponse, ProfileId> =
    fun (request:UnsubscribeRequest) ->
        Error (request.ProfileId |> ProfileId)

type ResultOf =
    | Follow      of Result<SubscriptionResponse, ProfileId>
    | Unsubscribe of Result<SubscriptionResponse, ProfileId>