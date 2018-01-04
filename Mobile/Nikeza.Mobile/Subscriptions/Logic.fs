module internal Are.Subscription

open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Try
open Nikeza.Common

type private FollowLogic =      Follow.ResultOf      -> NotificationEvent list
type private UnsubscribeLogic = Unsubscribe.ResultOf -> NotificationEvent list

module Follow =
    open Nikeza.Mobile.Subscriptions.Try.Follow

    let events : FollowLogic =
        fun command -> command |> function
        ResultOf.Follow response -> 
                        response |> function
                                    | Ok    info      -> [SubscriberAdded    <| ProfileId info.User.Profile.Id]
                                    | Error profileId -> [SubscriberAddFailed   profileId]

module Unsubscribe =
    open Nikeza.Mobile.Subscriptions.Try.Unsubscribe

    let events : UnsubscribeLogic =
        fun command -> command |> function
        | Unsubscribe response -> 
                      response |> function
                                  | Ok    info      -> [SubscriberRemoved   <| ProfileId info.User.Profile.Id]
                                  | Error profileId -> [SubscriberRemoveFailed profileId]