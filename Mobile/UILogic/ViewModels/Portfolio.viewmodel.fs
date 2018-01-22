namespace Nikeza.Mobile.UILogic.Portal.Portfolio

open System.Windows.Input
open Nikeza.Common
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Pages
open Nikeza.Mobile.UILogic.Publisher
open Nikeza.Mobile.Subscription
open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Try
open Nikeza.Mobile.Subscriptions.Command
open Nikeza.Mobile.Portfolio.Query

type Depedencies = {
        UserId :        ProviderId
        ProviderId :    ProviderId
        PortfolioFn :   PortfolioFn
        FollowFn :      FollowFn
        UnsubscribeFn : UnsubscribeFn
}

type ViewModel(injected) =

    inherit ViewModelBase()

    let mutable provider =  None

    let commandEvents = Event<NotificationEvent>()
    let pageRequested = Event<PageRequested>()

    let getId profileId = profileId |> function ProviderId id -> id
    
    let isAlreadyFollowing subscriberId =
        provider |> function
                    | Some p -> p.Followers |> List.contains subscriberId
                    | None -> false

    let articles() = injected.UserId |> PageRequested.Articles |> publishEvent pageRequested
    let videos() =   injected.UserId |> PageRequested.Videos   |> publishEvent pageRequested
    let answers() =  injected.UserId |> PageRequested.Answers  |> publishEvent pageRequested
    let podcasts() = injected.UserId |> PageRequested.Podcasts |> publishEvent pageRequested

    let follow() =
        { FollowRequest.SubscriberId= getId injected.UserId
          FollowRequest.ProfileId=    getId injected.ProviderId 
        } 
          |> Follow.Command.Execute
          |> Workflow.follow injected.FollowFn
          |> publishEvents commandEvents

    let unsubscribe() =
        { UnsubscribeRequest.SubscriberId= getId injected.UserId
          UnsubscribeRequest.ProfileId=    getId injected.ProviderId 
        } 
          |> Unsubscribe.Command.Execute
          |> Workflow.unsubscribe injected.UnsubscribeFn
          |> publishEvents commandEvents

    let followCommand =      DelegateCommand( (fun _ -> follow() ), 
                                               fun _ -> not <| isAlreadyFollowing (getId injected.UserId) ) 
                                               :> ICommand

    let unsubscribeCommand = DelegateCommand( (fun _ -> unsubscribe()), 
                                               fun _ -> isAlreadyFollowing <| getId injected.UserId ) 
                                               :> ICommand

    let articlesCommand = DelegateCommand( (fun _ -> articles()), fun _ -> true) :> ICommand
    let videosCommand =   DelegateCommand( (fun _ -> videos()),   fun _ -> true) :> ICommand
    let answersCommand =  DelegateCommand( (fun _ -> answers()),  fun _ -> true) :> ICommand
    let podcastsCommand = DelegateCommand( (fun _ -> podcasts()), fun _ -> true) :> ICommand

    member x.Articles = articlesCommand
    member x.Videos =   videosCommand
    member x.Answers =  answersCommand
    member x.Podcasts = podcastsCommand

    member x.Follow =      followCommand
    member x.Unsubscribe = unsubscribeCommand
    member x.Provider =    provider

    member x.Init() =

             injected.ProviderId 
              |> injected.PortfolioFn
              |> function
                 | Result.Ok     p -> provider <- Some p
                 | Result.Error id -> let error = { Context=id; Description="Failed to load portfolio" }
                                      pageRequested |> publishEvent <| Pages.PortfolioError error
                               
    member x.CommandEvents() = commandEvents.Publish
    member x.PageRequested() = pageRequested.Publish