namespace Nikeza.Mobile.UILogic.Portal.Portfolio

open System.Windows.Input
open Nikeza.Common
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Portfolio.Events
open Nikeza.Mobile.Subscription
open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Try
open Nikeza.Mobile.Subscriptions.Command
open Nikeza.Mobile.Portfolio.Query
open Nikeza.Mobile.UILogic.Response
open Nikeza.Mobile.UILogic.Pages

type Actions = {
    Follow      : FollowFn
    Unsubscribe : UnsubscribeFn
}

type Query = {
    Portfolio : PortfolioFn
}

type Observers = {
    ForFollow        : (NotificationEvent -> unit) list
    ForUnsubscribe   : (NotificationEvent -> unit) list
    ForQueryFailed   : (GetPortfolioEvent -> unit) list
    ForPageRequested : (PageRequested     -> unit) list
}

type Dependencies = {
    UserId     : ProviderId
    ProviderId : ProviderId
    Query      : Query
    Actions    : Actions
    Observers  : Observers
}

type ViewModel(dependencies:Dependencies) =

    inherit ViewModelBase()

    let userId =         dependencies.UserId
    let providerId =     dependencies.ProviderId
    let implementation = dependencies.Actions
    let responders =     dependencies.Observers
    let query =          dependencies.Query
    
    let mutable provider =  None

    let getId profileId = profileId |> function ProviderId id -> id
    
    let isAlreadyFollowing subscriberId =
        provider |> function
                    | Some p -> p.Followers |> List.contains subscriberId
                    | None -> false

    let broadcast events = 
        events |> List.iter (fun event -> responders.ForPageRequested |> handle event)

    let articles() = [userId |> PageRequested.Articles] |> broadcast
    let videos() =   [userId |> PageRequested.Videos  ] |> broadcast
    let answers() =  [userId |> PageRequested.Answers ] |> broadcast
    let podcasts() = [userId |> PageRequested.Podcasts] |> broadcast

    let follow() =

        let broadcast events = 
            events |> List.iter (fun event -> responders.ForFollow|> handle event)

        { FollowRequest.SubscriberId= getId userId
          FollowRequest.ProfileId=    getId providerId
        } 
          |> Follow.Command.Execute
          |> Workflow.follow implementation.Follow
          |> broadcast

    let unsubscribe() =

        let broadcast events = 
            events |> List.iter (fun event -> responders.ForUnsubscribe|> handle event)

        { UnsubscribeRequest.SubscriberId= getId userId
          UnsubscribeRequest.ProfileId=    getId providerId 
        } 
          |> Unsubscribe.Command.Execute
          |> Workflow.unsubscribe implementation.Unsubscribe
          |> broadcast

    let followCommand =      DelegateCommand( (fun _ -> follow() ), 
                                               fun _ -> not <| isAlreadyFollowing (getId userId) ) 
                                               :> ICommand

    let unsubscribeCommand = DelegateCommand( (fun _ -> unsubscribe()), 
                                               fun _ -> isAlreadyFollowing <| getId userId ) 
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

        let broadcast events = 
            events |> List.iter (fun event -> responders.ForQueryFailed|> handle event)

        providerId 
         |> query.Portfolio
         |> function
            | Result.Ok    p          -> provider <- Some p
            | Result.Error providerId -> let error = { Context=providerId; Description="Failed to load portfolio" }
                                         [GetPortfolioEventFailed error] |> broadcast