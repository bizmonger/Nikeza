namespace Nikeza.Mobile.UILogic.Portal.Recent

open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Publisher
open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Query
open Nikeza.Mobile.Portfolio.Events
open Nikeza.Mobile.Portfolio.Query
open Nikeza.Mobile.UILogic.Pages

type PortfolioEvent =     Nikeza.Mobile.Portfolio.Events.Query
type SubscriptionsEvent = Nikeza.Mobile.Subscriptions.Events.Query

type ViewModel(user:Provider, queryRecent:RecentFn, queryPortfolio:PortfolioFn) =

    inherit ViewModelBase()

    let pageRequested =      new Event<PageRequested>()
    let subscriptionsEvent = new Event<SubscriptionsEvent>()

    let mutable selection: Provider option = None
    let mutable recent:    Provider list =   []
    
    let viewProvider() =
        selection 
         |> function
            | Some provider -> 
                   provider.Profile.Id 
                    |> ProviderId  
                    |> queryPortfolio
                    |> function
                       | Query.Succeeded p  -> publishEvent pageRequested <| Portfolio p
                       | Query.Failed   id  -> publishEvent pageRequested <| PortfolioError { Context=id; Description="Failed to get portfolio" }
            | None -> ()

    member x.ViewProvider = DelegateCommand( (fun _ -> viewProvider() ), fun _ -> selection.IsSome)

    member x.Selection
             with get() =      selection
             and  set(value) = selection <- value

    member x.Providers
             with get() =      recent
             and  set(value) = recent    <- value

    member x.Init() =
        queryRecent <| ProfileId user.Profile.Id
         |> function
            | Query.RecentSucceeded providers -> recent <- providers
            | other -> publishEvent subscriptionsEvent other