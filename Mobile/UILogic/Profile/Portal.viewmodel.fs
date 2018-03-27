namespace Nikeza.Mobile.Portal

open System.Collections.ObjectModel
open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Subscriptions.Query

type Query = {
    Subscriptions : SubscriptionsFn
}

type SideEffects = {
    ForQueryFailed : (ProfileId error -> unit) list
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

    let mutable subscritions = ObservableCollection<Provider>()

    member x.Subscriptions
             with get() =      subscritions
             and  set(value) = subscritions <- value
                               base.NotifyPropertyChanged(<@ x.Subscriptions @>)

    member x.Init() = 

        let broadcast (events:error<ProfileId> list) = 
            events |> List.iter (fun event -> sideEffects.ForQueryFailed |> handle event)
            
        userId
         |> query.Subscriptions
         |> function
            | Ok    result -> x.Subscriptions <- ObservableCollection<Provider>(result)
            | Error msg    -> broadcast [msg]