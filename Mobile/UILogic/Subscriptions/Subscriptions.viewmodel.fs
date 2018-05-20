namespace Nikeza.Mobile.UILogic.Portal.Subscriptions

open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Query
open Nikeza.Mobile.Portfolio.Query
open Nikeza.Mobile.UILogic.Pages

type Query = {
    Portfolio     : PortfolioFn
    Subscriptions : SubscriptionsFn
}

type SideEffects = {
    ForPageRequested : (PageRequested    -> unit) nonempty
    ForQueryFailed   : (GetProfilesEvent -> unit) nonempty
}

type Dependencies = {
    UserId      : ProfileId
    Query       : Query
    SideEffects : SideEffects
}

type ViewModel(dependencies) =

    inherit ViewModelBase()

    let userId =      dependencies.UserId
    let query  =      dependencies.Query
    let sideEffects = dependencies.SideEffects

    let mutable selection:     Provider option = None
    let mutable subscriptions: Provider list =   []
    
    let viewProvider() =

        let broadcast event = 
            sideEffects.ForPageRequested |> handle event

        selection |> function
                     | Some provider -> 
                            provider.Profile.Id 
                             |> ProviderId  
                             |> query.Portfolio
                             |> function
                                | Result.Ok    p          -> broadcast <| PageRequested.Portfolio p.Profile
                                | Result.Error providerId -> broadcast <| PageRequested.Error { Context=providerId |> string; Description="Failed to load portfolio" }
                     | None -> ()

    member x.ViewProvider = DelegateCommand( (fun _ -> viewProvider() ), fun _ -> selection.IsSome)

    member x.Selection
             with get() =      selection
             and  set(value) = selection     <- value

    member x.Providers
             with get() =      subscriptions
             and  set(value) = subscriptions <- value

    member x.Init() =

        let broadcast events = 
            events.Head::events.Tail |> List.iter (fun event -> sideEffects.ForQueryFailed |> handle event)
        
        query.Subscriptions userId
         |> function
            | Result.Ok providers -> subscriptions <- providers
            | Result.Error error  -> broadcast { Head= GetSubscriptionsFailed error; Tail= [] }