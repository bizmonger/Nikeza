namespace Nikeza.Mobile.UILogic.Portal.Recent

open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Publisher
open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Query
open Nikeza.Mobile.Portfolio

type PortfolioEvent =     Nikeza.Mobile.Portfolio.Events.GetPortfolioEvent
type SubscriptionsEvent = Nikeza.Mobile.Subscriptions.Events.QueryEvent

type ViewModel(user:Provider, recentFn:RecentFn) =

    inherit ViewModelBase()

    let portfolioEvent =     new Event<PortfolioEvent>()
    let subscriptionsEvent = new Event<SubscriptionsEvent>()

    let mutable selection: Provider option = None
    let mutable recent:    Provider list =   []
    
    let viewProvider() =
        selection |> function
                     | Some provider -> provider.Profile.Id 
                                         |> ProviderId  
                                         |> Query.portfolio
                                         |> publishEvent portfolioEvent
                     | None -> ()

    member x.ViewProvider = DelegateCommand( (fun _ -> viewProvider() ), fun _ -> selection.IsSome)

    member x.Selection
             with get() =      selection
             and  set(value) = selection <- value

    member x.Providers
             with get() =      recent
             and  set(value) = recent    <- value

    member x.Load() =
        recentFn <| ProfileId user.Profile.Id
         |> function
            | GetRecentSucceeded providers :: [] -> recent <- providers
            | other -> publishEvents subscriptionsEvent other