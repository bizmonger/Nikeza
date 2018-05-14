namespace Nikeza.Mobile.UILogic.Portal

open System.Collections.ObjectModel
open System.Windows.Input
open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Subscriptions.Query
open Nikeza.Mobile.UILogic.Pages

type RecentLinksAdapter(profileSeed) as x =

    inherit ObservableCollection<Link list>()
    let mutable profile =     profileSeed
    let mutable recentLinks = x

    member x.Profile 
        with get()=       profile
        and  set(value) = profile <- value
                
    member x.RecentLinks
        with get() =      recentLinks
        and  set(value) = recentLinks <- value

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
    let mutable subscritions = ObservableCollection<RecentLinksAdapter>()

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

        let setSubscriptions (result:ProviderRequest list) =

            let toRecentLinks (s:ProviderRequest) =

                let recentLinksList = RecentLinksAdapter(s.Profile)
                recentLinksList.Add(s.RecentLinks)
                recentLinksList

            let adapter = result |> List.map toRecentLinks
            x.Subscriptions <- ObservableCollection<RecentLinksAdapter>(adapter)
            
        userId
         |> query.Subscriptions
         |> function
            | Result.Ok    result -> setSubscriptions result
            | Result.Error msg    -> broadcast [msg]