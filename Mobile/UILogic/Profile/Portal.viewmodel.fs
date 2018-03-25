namespace Nikeza.Mobile.Portal

open System.Diagnostics
open System.Collections.ObjectModel
open Nikeza.Common
open Nikeza.Mobile.Profile.Language
open Nikeza.Mobile.Profile.Events
open Nikeza.Mobile.Profile.Queries
open Nikeza.Mobile.UILogic

type Query = {
    Subscriptions : SubscriptionsFn
}

type SideEffects = {
    ForSubscriptionsQuery : (QuerySubscriptionsEvent -> unit) list
}

type Dependencies = {
    UserId      : ProfileId
    Query       : Query
    SideEffects : SideEffects
}

type ViewModel(dependencies) as x =

    inherit ViewModelBase()

    let userId=       dependencies.UserId
    let query=        dependencies.Query
    let sideEffects'= dependencies.SideEffects

    let onQueryResponse = function
        | QuerySucceeeded result -> x.Subscriptions <- ObservableCollection<Subscription>(result)
        | QueryFailed id         -> Debug.WriteLine(sprintf "Failed to retrieve subscriptions with userid: %A" id)

    let sideEffects = 
        { sideEffects' with ForSubscriptionsQuery= onQueryResponse::sideEffects'.ForSubscriptionsQuery }

    let broadcast events = 
        events |> List.iter (fun event -> sideEffects.ForSubscriptionsQuery |> handle event)

    let mutable subscritions = ObservableCollection<Subscription>()

    member x.Subscriptions
             with get() =      subscritions
             and  set(value) = subscritions <- value
                               base.NotifyPropertyChanged(<@ x.Subscriptions @>)

    member x.Init() = 
           userId
            |> query.Subscriptions
            |> broadcast