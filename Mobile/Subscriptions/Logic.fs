module internal Are

open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Attempt
open Nikeza.Common

type private FollowLogic =      Follow.ResultOf      -> NotificationEvent list
type private UnsubscribeLogic = Unsubscribe.ResultOf -> NotificationEvent list

module Follow =
    open Nikeza.Mobile.Subscriptions.Attempt.Follow

    let events : FollowLogic = function
         ResultOf.Follow response -> 
                         response |> function
                                     | Ok    info      -> [SubscriberAdded    <| ProfileId info.User.Profile.Id]
                                     | Error profileId -> [SubscriberAddFailed   profileId]

module Unsubscribe =
    open Nikeza.Mobile.Subscriptions.Attempt.Unsubscribe

    let events : UnsubscribeLogic = function
         Unsubscribe response -> 
                     response |> function
                                 | Ok    info      -> [SubscriberRemoved   <| ProfileId info.User.Profile.Id]
                                 | Error profileId -> [SubscriberRemoveFailed profileId]