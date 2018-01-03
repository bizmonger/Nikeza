namespace Nikeza.Mobile.UILogic.Portal.Recent

open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic.Publisher
open Nikeza.Mobile.Subscription.Events
open Nikeza.Mobile.Subscriptions.Query
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Portfolio

type PortfolioEvent =     Nikeza.Mobile.Portfolio.Events.QueryEvent
type SubscriptionsEvent = Nikeza.Mobile.Subscription.Events.QueryEvent

type ViewModel(user:Provider, getRecent:RecentFn) =

    let portfolioEvent =     new Event<PortfolioEvent>()
    let subscriptionsEvent = new Event<SubscriptionsEvent>()

    let mutable selection: Provider option = None
    let mutable latest:    Provider list =   []
    
    let viewProvider() =
        selection |> function
                     | Some provider -> provider.Profile.Id 
                                         |> ProviderId  
                                         |> Query.portfolio
                                         |> publish portfolioEvent
                     | None -> ()

    member x.ViewProvider = DelegateCommand( (fun _ -> viewProvider() ), fun _ -> selection.IsSome)

    member x.Selection
             with get() =      selection
             and  set(value) = selection <- value

    member x.Providers
             with get() =      latest
             and  set(value) = latest    <- value

    member x.Load() =
        getRecent <| ProfileId user.Profile.Id
         |> function
            | GetLatestSucceeded providers :: [] -> latest <- providers
            | otherEvents -> publish subscriptionsEvent otherEvents