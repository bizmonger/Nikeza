namespace Nikeza.Mobile.UILogic.Portal.Members

open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Publisher
open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Query
open Nikeza.Mobile.Portfolio

type SubscriptionsEvent = Nikeza.Mobile.Subscriptions.Events.MembersQuery

type ViewModel(user:Provider, getMembers:MembersFn) =

    inherit ViewModelBase()

    let portfolioResult =     new Event<_>()
    let subscriptionsEvent = new Event<SubscriptionsEvent>()

    let mutable selection: Provider option = None
    let mutable members:   Provider list =   []
    
    let viewProvider() =
        selection |> function
                     | Some provider -> provider.Profile.Id 
                                         |> ProviderId  
                                         |> Query.portfolio
                                         |> publishEvent portfolioResult
                     | None -> ()

    member x.ViewProvider = DelegateCommand( (fun _ -> viewProvider() ), fun _ -> selection.IsSome)

    member x.Selection
             with get() =      selection
             and  set(value) = selection <- value

    member x.Providers
             with get() =      members
             and  set(value) = members   <- value

    member x.Init() =
             getMembers() |> function
                             | MembersSucceeded providers -> members <- providers
                             | other -> publishEvent subscriptionsEvent other