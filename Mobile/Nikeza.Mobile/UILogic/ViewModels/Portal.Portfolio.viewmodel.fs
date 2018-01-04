namespace Nikeza.Mobile.UILogic.Portal.Portfolio

open System.Windows.Input
open Nikeza.Common
open Nikeza.Mobile.Portfolio.Events
open Nikeza.Mobile.Portfolio.Query
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Publisher
open Nikeza.Mobile.Subscription
open Nikeza.Mobile.Subscription.Commands
open Nikeza.Mobile.Subscription.Events

type PortfolioQuery = Nikeza.Mobile.Portfolio.Events.QueryEvent

type ViewModel(userId, providerId, getPortfolio) =

    let mutable provider =  uninitializedProvider
    let eventsFromQuery =   Event<PortfolioQuery>()
    let eventsFromCommand = Event<NotificationEvent>()

    let getId profileId = profileId |> function ProviderId id -> id
    
    let isAlreadyFollowing subscriberId =
        provider.Followers |> List.contains subscriberId

    let follow() =
        { FollowRequest.SubscriberId= getId userId
          FollowRequest.ProfileId=    getId providerId 
        } 
           |> Command.Follow 
           |> Execute.Subscriptions.workflow 
           |> publish eventsFromCommand

    let unsubscribe() =
        { UnsubscribeRequest.SubscriberId= getId userId
          UnsubscribeRequest.ProfileId=    getId providerId 
        } 
           |> Command.Unsubscribe
           |> Execute.Subscriptions.workflow 
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
                 | GetPortfolioSucceeded p :: _ -> provider <- p
                 | otherEvents -> otherEvents |> publish eventsFromQuery

    member x.QueryEvents() =   eventsFromQuery.Trigger
    member x.CommandEvents() = eventsFromCommand.Trigger