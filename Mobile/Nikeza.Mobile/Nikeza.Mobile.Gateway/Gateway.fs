module Nikeza.Mobile.WebRequests

open Nikeza.Common
open Nikeza.DataTransfer

let tryFollow      (request:FollowRequest) :      Result<SubscriptionResponse, ProfileId> = 
    Error (request.ProfileId |> ProfileId)

let tryUnsubscribe (request:UnsubscribeRequest) : Result<SubscriptionResponse, ProfileId> = 
    Error (request.ProfileId |> ProfileId)