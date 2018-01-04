namespace Nikeza.Mobile.UILogic.Portal.Portfolio

open System.Windows.Input
open Nikeza.Common
open Nikeza.Mobile.Portfolio.Events
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Publisher
open Nikeza.Mobile.Subscription
open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Try
open Nikeza.Mobile.Subscriptions.Command
open Nikeza.Mobile.Portfolio.Query

type PortfolioQuery = Nikeza.Mobile.Portfolio.Events.QueryEvent

type Depedencies = {
    UserId :        ProviderId
    ProviderId :    ProviderId
    PortfolioFn :  PortfolioFn
    FollowFn :      FollowFn
    UnsubscribeFn : UnsubscribeFn
}

type ViewModel(injected) =

    let mutable provider =  None
    let eventsFromQuery =   Event<PortfolioQuery>()
    let commandEvents = Event<NotificationEvent>()

    let getId profileId = profileId |> function ProviderId id -> id
    
    let isAlreadyFollowing subscriberId =
        provider |> function
                    | Some p -> p.Followers |> List.contains subscriberId
                    | None -> false

    let follow() =
        { FollowRequest.SubscriberId= getId injected.UserId
          FollowRequest.ProfileId=    getId injected.ProviderId 
        } 
          |> Follow.Command.Execute
          |> Workflow.follow injected.FollowFn
          |> publish commandEvents

    let unsubscribe() =
        { UnsubscribeRequest.SubscriberId= getId injected.UserId
          UnsubscribeRequest.ProfileId=    getId injected.ProviderId 
        } 
          |> Unsubscribe.Command.Execute
          |> Workflow.unsubscribe injected.UnsubscribeFn
          |> publish commandEvents

    let followCommand =      DelegateCommand( (fun _ -> follow() ), 
                                               fun _ -> not <| isAlreadyFollowing (getId injected.UserId) ) 
                                               :> ICommand

    let unsubscribeCommand = DelegateCommand( (fun _ -> unsubscribe()), 
                                               fun _ -> isAlreadyFollowing <| getId injected.UserId ) 
                                               :> ICommand
    member x.Follow =      followCommand
    member x.Unsubscribe = unsubscribeCommand
    member x.Provider =    provider

    member x.Load() =
             injected.ProviderId 
              |> injected.PortfolioFn
              |> function
                 | GetPortfolioSucceeded p :: _ -> provider <- Some p
                 | otherEvents -> otherEvents |> publish eventsFromQuery

    member x.QueryEvents() =   eventsFromQuery.Publish
    member x.CommandEvents() = commandEvents.Publish