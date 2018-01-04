namespace Nikeza.Mobile.UILogic.Portal.Subscriptions

open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic.Publisher
open Nikeza.Mobile.Subscription.Events
open Nikeza.Mobile.Subscriptions.Query
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Portfolio

type PortfolioEvent =     Nikeza.Mobile.Portfolio.Events.QueryEvent
type SubscriptionsEvent = Nikeza.Mobile.Subscription.Events.QueryEvent

type ViewModel(user:Provider, getSubscriptions:SubscriptionsFn) =

    let portfolioEvent =     new Event<PortfolioEvent>()
    let subscriptionsEvent = new Event<SubscriptionsEvent>()

    let mutable selection:     Provider option = None
    let mutable subscriptions: Provider list =   []
    
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
             and  set(value) = selection     <- value

    member x.Providers
             with get() =      subscriptions
             and  set(value) = subscriptions <- value

    member x.Load() =
             getSubscriptions <| ProfileId user.Profile.Id
              |> function
                 | GetSubscriptionsSucceeded providers :: _ -> subscriptions <- providers
                 | otherEvents -> publish subscriptionsEvent otherEvents