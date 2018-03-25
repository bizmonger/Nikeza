namespace Nikeza.Mobile.Portal

open System.Collections.ObjectModel
open Nikeza.Common
open Nikeza.Mobile.Profile.Language
open Nikeza.Mobile.Profile.Queries
open Nikeza.Mobile.UILogic

type Query = {
    Subscriptions : SubscriptionsFn
}

type SideEffects = {
    ForSubscriptionsQuery : (Result<Subscription list,ProfileId> -> unit) list
}

type Dependencies = {
    UserId      : ProfileId
    Query       : Query
    SideEffects : SideEffects
}

type ViewModel(dependencies) =

    inherit ViewModelBase()

    let userId=      dependencies.UserId
    let query=       dependencies.Query
    let sideEffects= dependencies.SideEffects

    let mutable subscritions = ObservableCollection<Subscription>()

    member x.Subscriptions
             with get() =      subscritions
             and  set(value) = subscritions <- value
                               base.NotifyPropertyChanged(<@ x.Subscriptions @>)

    member x.Init() = 

        let broadcast events = 
            events |> List.iter (fun event -> sideEffects.ForSubscriptionsQuery |> handle event)
            
        userId
         |> query.Subscriptions
         |> function
            | Ok    result -> x.Subscriptions <- ObservableCollection<Subscription>(result)
            | Error msg    -> broadcast [Error msg]