namespace Nikeza.Mobile.UILogic.Portal.Members

open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Query
open Nikeza.Mobile.Portfolio
open Nikeza.Mobile.UILogic.Pages

type Query = {
    Members : MembersFn
}

type SideEffects = {
    ForPageRequested : (PageRequested    -> unit) nonempty
    ForQueryFailed   : (GetProfilesEvent -> unit) nonempty
}

type Dependencies = {
    Query       : Query
    SideEffects : SideEffects
}

type ViewModel(dependencies) =

    inherit ViewModelBase()

    let sideEffects = dependencies.SideEffects
    let query =       dependencies.Query

    let mutable selection: Provider option = None
    let mutable members:   Provider list =   []
    
    let viewProvider() =

        let broadcast event = 
            sideEffects.ForPageRequested |> handle event

        selection |> function
                     | Some provider -> provider.Profile.Id 
                                         |> ProviderId  
                                         |> Query.portfolio
                                         |> function
                                            | Result.Ok    p           -> broadcast <| PageRequested.Portfolio p
                                            | Result.Error providerId  -> broadcast <| PageRequested.PortfolioError { Context=providerId; Description="Failed to get portfolio" }
                     | None -> ()

    member x.ViewProvider = DelegateCommand( (fun _ -> viewProvider() ), fun _ -> selection.IsSome)

    member x.Selection
             with get() =      selection
             and  set(value) = selection <- value

    member x.Providers
             with get() =      members
             and  set(value) = members   <- value

    member x.Init() =

        let broadcast events = 
            events |> List.iter (fun event -> sideEffects.ForQueryFailed |> handle event)

        query.Members()
         |> function
            | Result.Ok    providers -> members <- providers
            | Result.Error profileId -> [GetMembersFailed profileId] |> broadcast