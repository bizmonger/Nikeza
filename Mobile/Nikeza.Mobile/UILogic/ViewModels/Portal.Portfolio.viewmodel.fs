namespace Nikeza.Mobile.UILogic.Portal.Portfolio

open System.Windows.Input
open Nikeza.Common
open Nikeza.Mobile.Portfolio.Events
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Publisher
open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Try
open Nikeza.Mobile.Subscriptions.Command
open Nikeza.Mobile.Subscription.Subscriptions

type PortfolioQuery = Nikeza.Mobile.Portfolio.Events.QueryEvent

type ViewModel(userId, providerId, getPortfolio, followFn:FollowFn, unsubscribeFn:UnsubscribeFn) =

    let mutable provider =  None
    let eventsFromQuery =   Event<PortfolioQuery>()
    let eventsFromCommand = Event<NotificationEvent>()

    let getId profileId = profileId |> function ProviderId id -> id
    
    let isAlreadyFollowing subscriberId =
        provider |> function
                    | Some p -> p.Followers |> List.contains subscriberId
                    | None -> false

    let follow() =
        { FollowRequest.SubscriberId= getId userId
          FollowRequest.ProfileId=    getId providerId 
        } 
           |> Follow.Command.Execute
           |> Workflow.follow followFn
           |> publish eventsFromCommand

    let unsubscribe() =
        { UnsubscribeRequest.SubscriberId= getId userId
          UnsubscribeRequest.ProfileId=    getId providerId 
        } 
           |> Unsubscribe.Command.Execute
           |> Workflow.unsubscribe unsubscribeFn
           |> publish eventsFromCommand

    let followCommand =      DelegateCommand( (fun _ -> follow() ), 
                                               fun _ -> not <| isAlreadyFollowing (getId userId) ) 
                                               :> ICommand

    let unsubscribeCommand = DelegateCommand( (fun _ -> unsubscribe()), 
                                               fun _ -> isAlreadyFollowing <| getId userId ) 
                                               :> ICommand
    member x.Follow =      followCommand
    member x.Unsubscribe = unsubscribeCommand
    member x.Provider =    provider

    member x.Load() =
             providerId 
              |> getPortfolio
              |> function
                 | GetPortfolioSucceeded p :: _ -> provider <- Some p
                 | otherEvents -> otherEvents |> publish eventsFromQuery

    member x.QueryEvents() =   eventsFromQuery.Publish
    member x.CommandEvents() = eventsFromCommand.Publish