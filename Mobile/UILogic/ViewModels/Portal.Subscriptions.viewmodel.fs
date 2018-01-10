namespace Nikeza.Mobile.UILogic.Portal.Subscriptions

open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Publisher
open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Query
open Nikeza.Mobile.Portfolio

type PortfolioEvent =     Nikeza.Mobile.Portfolio.Events.Query
type SubscriptionsEvent = Nikeza.Mobile.Subscriptions.Events.QueryEvent

type ViewModel(user:Provider, getSubscriptions:SubscriptionsFn) =

    inherit ViewModelBase()

    let portfolioEvent =     new Event<PortfolioEvent>()
    let subscriptionsEvent = new Event<SubscriptionsEvent>()

    let mutable selection:     Provider option = None
    let mutable subscriptions: Provider list =   []
    
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
             and  set(value) = selection     <- value

    member x.Providers
             with get() =      subscriptions
             and  set(value) = subscriptions <- value

    member x.Load() =
             getSubscriptions <| ProfileId user.Profile.Id
              |> function
                 | GetSubscriptionsSucceeded providers :: _ -> subscriptions <- providers
                 | otherEvents -> publishEvents subscriptionsEvent otherEvents