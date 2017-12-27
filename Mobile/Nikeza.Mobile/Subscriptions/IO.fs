module IO

open Nikeza.Common
open Nikeza.DataTransfer

let tryFollow (request:FollowRequest) :           Result<SubscriptionResponse, ProfileId> = 
    Error (request.ProfileId |> ProfileId)

let tryUnsubscribe (request:UnsubscribeRequest) : Result<SubscriptionResponse, ProfileId> = 
    Error (request.ProfileId |> ProfileId)

type ResultOf =
    | Follow      of Result<SubscriptionResponse, ProfileId>
    | Unsubscribe of Result<SubscriptionResponse, ProfileId>