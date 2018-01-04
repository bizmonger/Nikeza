namespace Nikeza.Mobile.UILogic.Portal.Members

open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic.Publisher
open Nikeza.Mobile.Subscription.Events
open Nikeza.Mobile.Subscriptions.Query
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Portfolio

type PortfolioEvent =     Nikeza.Mobile.Portfolio.Events.QueryEvent
type SubscriptionsEvent = Nikeza.Mobile.Subscription.Events.QueryEvent

type ViewModel(user:Provider, getMembers:MembersFn) =

    let portfolioEvent =     new Event<PortfolioEvent>()
    let subscriptionsEvent = new Event<SubscriptionsEvent>()

    let mutable selection: Provider option = None
    let mutable members:   Provider list =   []
    
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
             with get() =      members
             and  set(value) = members   <- value

    member x.Load() =
             getMembers() |> function
                            | GetMembersSucceeded providers :: _ -> members <- providers
                            | otherEvents -> publish subscriptionsEvent otherEvents