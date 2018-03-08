namespace Nikeza.Mobile.UILogic.Portal.Members

open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Query
open Nikeza.Mobile.Portfolio
open Nikeza.Mobile.UILogic.Pages
open Nikeza.Mobile.UILogic.Response

type Query = {
    Members : MembersFn
}

type Observers = {
    ForPageRequested : (PageRequested  -> unit) list
    ForQueryFailed   : (GetProfilesEvent -> unit) list
}

type Dependencies = {
    Query     : Query
    Observers : Observers
}

type ViewModel(dependencies:Dependencies) =

    inherit ViewModelBase()

    let responders = dependencies.Observers
    let query =      dependencies.Query

    let mutable selection: Provider option = None
    let mutable members:   Provider list =   []
    
    let viewProvider() =

        let broadcast (events:PageRequested list) = 
            events |> List.iter (fun event -> responders.ForPageRequested |> handle event)

        selection |> function
                     | Some provider -> provider.Profile.Id 
                                         |> ProviderId  
                                         |> Query.portfolio
                                         |> function
                                            | Result.Ok    p           -> broadcast [PageRequested.Portfolio p]
                                            | Result.Error providerId  -> let error = { Context=providerId; Description="Failed to get portfolio" }
                                                                          broadcast [PageRequested.PortfolioError error]
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
            events |> List.iter (fun event -> responders.ForQueryFailed |> handle event)

        query.Members()
         |> function
            | Result.Ok    providers -> members <- providers
            | Result.Error profileId -> [GetMembersFailed profileId] |> broadcast