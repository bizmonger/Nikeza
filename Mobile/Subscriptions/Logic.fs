namespace Nikeza.Mobile.Subscriptions

open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Common
open Nikeza.DataTransfer

type private FollowLogic =      Result<SubscriptionResponse, ProfileId> -> NotificationEvent list
type private UnsubscribeLogic = Result<SubscriptionResponse, ProfileId> -> NotificationEvent list

module FollowResponse =

    let toEvents : FollowLogic = function
         
        response -> 
        response |> function
                    | Ok    info      -> [SubscriberAdded    <| ProfileId info.User.Profile.Id]
                    | Error profileId -> [SubscriberAddFailed   profileId]

module UnsubscribeResponse =

    let toEvents : UnsubscribeLogic = function
         
        response -> 
        response |> function
                    | Ok    info      -> [SubscriberRemoved   <| ProfileId info.User.Profile.Id]
                    | Error profileId -> [SubscriberRemoveFailed profileId]