﻿namespace Nikeza.Mobile.UILogic.Portal

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
    User        : Profile
    Query       : Query
    SideEffects : SideEffects
}

type ViewModel(dependencies) =

    inherit ViewModelBase()

    let userId=      ProfileId dependencies.User.Id
    let user=        dependencies.User
    let query=       dependencies.Query
    let sideEffects= dependencies.SideEffects

    let mutable profileImage = ""
    let mutable subscritions = ObservableCollection<Provider>()

    let broadcast pageRequest = 
        sideEffects.ForPageRequested |> handle' pageRequest

    member x.ViewMembers =       DelegateCommand( (fun _-> broadcast    PageRequested.Members),            fun _ -> true) :> ICommand
    member x.ViewLatest =        DelegateCommand( (fun _-> broadcast <| PageRequested.Latest        user), fun _ -> true) :> ICommand
    member x.ViewFollowers =     DelegateCommand( (fun _-> broadcast <| PageRequested.Followers     user), fun _ -> true) :> ICommand
    member x.ViewSubscriptions = DelegateCommand( (fun _-> broadcast <| PageRequested.Subscriptions user), fun _ -> true) :> ICommand

    member x.Name = sprintf "%s %s" user.FirstName user.LastName

    member x.ProfileImage
             with get() =      profileImage
             and  set(value) = profileImage <- value
                               base.NotifyPropertyChanged(<@ x.ProfileImage @>)

    member x.Subscriptions
             with get() =      subscritions
             and  set(value) = subscritions <- value
                               base.NotifyPropertyChanged(<@ x.Subscriptions @>)

    member x.Init() =

        x.ProfileImage <- user.ImageUrl

        let broadcast (events:error<ProfileId> list) = 
            events |> List.iter (fun event -> sideEffects.ForQueryFailed |> handle' event)
            
        userId
         |> query.Subscriptions
         |> function
            | Result.Ok    result -> x.Subscriptions <- ObservableCollection<Provider>(result)
            | Result.Error msg    -> broadcast [msg]