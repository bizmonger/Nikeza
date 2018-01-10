namespace Nikeza.Mobile.UILogic.Portal.Members

open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Publisher
open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Query
open Nikeza.Mobile.Portfolio

type PortfolioEvent =     Nikeza.Mobile.Portfolio.Events.Query
type SubscriptionsEvent = Nikeza.Mobile.Subscriptions.Events.Query

type ViewModel(user:Provider, getMembers:MembersFn) =

    inherit ViewModelBase()

    let portfolioEvent =     new Event<PortfolioEvent>()
    let subscriptionsEvent = new Event<SubscriptionsEvent>()

    let mutable selection: Provider option = None
    let mutable members:   Provider list =   []
    
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
             with get() =      members
             and  set(value) = members   <- value

    member x.Load() =
             getMembers() |> function
                             | Query.MembersSucceeded providers -> members <- providers
                             | other -> publishEvent subscriptionsEvent other