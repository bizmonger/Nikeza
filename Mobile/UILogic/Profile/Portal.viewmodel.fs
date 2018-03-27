namespace Nikeza.Mobile.Portal

open System.Collections.ObjectModel
open System.Windows.Input
open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Subscriptions.Query
open Nikeza.Mobile.UILogic.Pages

type Query = {
    Subscriptions : SubscriptionsFn
}

type SideEffects = {
    ForQueryFailed   : (ProfileId error -> unit) list
    ForPageRequested : (PageRequested   -> unit) list
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

    member x.ViewMembers =       DelegateCommand( (fun _-> ()), fun _ -> true) :> ICommand
    member x.ViewRecent =        DelegateCommand( (fun _-> ()), fun _ -> true) :> ICommand
    member x.ViewFollowers =     DelegateCommand( (fun _-> ()), fun _ -> true) :> ICommand
    member x.ViewSubscriptions = DelegateCommand( (fun _-> ()), fun _ -> true) :> ICommand

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
            | Result.Ok    result -> x.Subscriptions <- ObservableCollection<Provider>(result)
            | Result.Error msg    -> broadcast [msg]