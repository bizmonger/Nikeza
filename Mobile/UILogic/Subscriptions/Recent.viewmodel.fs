namespace Nikeza.Mobile.UILogic.Portal.Recent

open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Subscriptions.Query
open Nikeza.Mobile.Portfolio.Query
open Nikeza.Mobile.UILogic.Pages
open Nikeza.Mobile.Subscriptions.Events

type Query = {
    Recent    : RecentFn
    Portfolio : PortfolioFn
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
    let sideEffects = dependencies.SideEffects
    let query =       dependencies.Query

    let mutable selection: Provider option = None
    let mutable recent:    Provider list =   []
    
    let viewProvider() =

        let broadcast event = 
            sideEffects.ForPageRequested |> handle event

        selection 
         |> function
            | Some provider -> 
                   provider.Profile.Id 
                    |> ProviderId  
                    |> query.Portfolio
                    |> function
                       | Result.Ok    p           -> broadcast <|  PageRequested.Portfolio p
                       | Result.Error providerId  -> broadcast <|  PageRequested.PortfolioError { Context=providerId; Description="Failed to get portfolio" }
            | None -> ()

    member x.ViewProvider = DelegateCommand( (fun _ -> viewProvider() ), fun _ -> selection.IsSome)

    member x.Selection
             with get() =      selection
             and  set(value) = selection <- value

    member x.Providers
             with get() =      recent
             and  set(value) = recent    <- value

    member x.Init() =

        let broadcast events = 
            events |> List.iter (fun event -> sideEffects.ForQueryFailed |> handle event)

        query.Recent userId
         |> function
            | Result.Ok    providers -> recent <- providers
            | Result.Error error     -> broadcast [GetRecentFailed error]