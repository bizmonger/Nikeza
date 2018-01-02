namespace Nikeza.Mobile.UILogic.Portal.Recent

open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.Subscriptions
open Nikeza.Mobile.UILogic.Publisher
open Nikeza.Mobile.Subscription.Events
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Portfolio

type PortfolioEvent =     Nikeza.Mobile.Portfolio.Events.QueryEvent
type SubscriptionsEvent = Nikeza.Mobile.Subscription.Events.QueryEvent

type ViewModel(user:Provider) as x =

    let portfolioEvent =     new Event<PortfolioEvent>()
    let subscriptionsEvent = new Event<SubscriptionsEvent>()

    let selection: Provider option = None

    let mutable latest:Provider list = []
    
    let view() =
        selection |> function
                     | Some provider -> provider.Profile.Id 
                                         |>ProviderId  
                                         |> Query.portfolio
                                         |> publish portfolioEvent
                     | None -> ()

    member x.View = DelegateCommand( (fun _ -> view() ), fun _ -> selection.IsSome)

    member x.User =      user
    member x.Selection = selection
    member x.Latest =    latest

    member x.Load() =
        Query.latest <| ProfileId user.Profile.Id
         |> function
            | GetLatestSucceeded providers :: [] -> latest <- providers
            | otherEvents -> publish subscriptionsEvent otherEvents