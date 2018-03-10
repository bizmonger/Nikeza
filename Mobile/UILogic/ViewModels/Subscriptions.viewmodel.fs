namespace Nikeza.Mobile.UILogic.Portal.Subscriptions

open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Subscriptions.Events
open Nikeza.Mobile.Subscriptions.Query
open Nikeza.Mobile.Portfolio.Query
open Nikeza.Mobile.UILogic.Pages
open Nikeza.Mobile.UILogic.Response


type Query = {
    Portfolio     : PortfolioFn
    Subscriptions : SubscriptionsFn
}

type Observers = {
    ForPageRequested : (PageRequested    -> unit) list
    ForQueryFailed   : (GetProfilesEvent -> unit) list
}

type Dependencies = {
    UserId    : ProfileId
    Query     : Query
    Observers : Observers
}

type ViewModel(dependencies) =

    inherit ViewModelBase()

    let userId =     dependencies.UserId
    let query  =     dependencies.Query
    let responders = dependencies.Observers

    let mutable selection:     Provider option = None
    let mutable subscriptions: Provider list =   []
    
    let viewProvider() =

        let broadcast (events:PageRequested list) = 
            events |> List.iter (fun event -> responders.ForPageRequested |> handle event)

        selection |> function
                     | Some provider -> provider.Profile.Id 
                                         |> ProviderId  
                                         //|> Query.portfolio
                                           |> query.Portfolio
                                                    |> function
                                                       | Result.Ok    p          -> broadcast [PageRequested.Portfolio p]
                                                       | Result.Error providerId -> let error = { Context=providerId |> string; Description="Failed to load portfolio" }
                                                                                    broadcast [PageRequested.Error error]
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
            events |> List.iter (fun event -> responders.ForQueryFailed |> handle event)
        
        query.Subscriptions userId
         |> function
            | Result.Ok providers -> subscriptions <- providers
            | Result.Error error  -> broadcast [GetSubscriptionsFailed error]