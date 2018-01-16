namespace Nikeza.Mobile.Portal.DataSources

open System
open System.Collections.ObjectModel
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Profile.Events
open Nikeza.Mobile.Profile
open Nikeza.Common

type ViewModel(platformsFn) as x =

    inherit ViewModelBase()

    let mutable platforms = ObservableCollection<string>()
    let mutable platform =  ""
    let mutable accessId =  ""
    let mutable sources =   ObservableCollection<string>()

    let canAdd() =
        not <| String.IsNullOrEmpty x.Platform &&
        not <| String.IsNullOrEmpty x.AccessId

    let add =    DelegateCommand( (fun _ -> () (*todo...*)) , fun _ -> canAdd() )
    let remove = DelegateCommand( (fun _ -> () (*todo...*)) , fun _ -> true )

    member x.Platform
             with get() =      platform
             and  set(value) = platform <- value
                               base.NotifyPropertyChanged(<@ x.Platform @>)

    member x.AccessId
             with get() =      accessId
             and  set(value) = accessId <- value
                               base.NotifyPropertyChanged(<@ x.AccessId @>)

    member x.Sources
             with get() =      sources
             and  set(value) = sources <- value
                               base.NotifyPropertyChanged(<@ x.Sources @>)

    member x.Platforms
             with get() =      platforms
             and  set(value) = platforms <- value
                               base.NotifyPropertyChanged(<@ x.Platforms @>)

    member x.Init() =

             let namesOf platforms = 
                platforms |> List.map(fun (Platform name) -> name)

             Query.platforms()
              |> function
                 | PlatformsSucceeded p -> platforms <- ObservableCollection<string>(namesOf p)
                 | PlatformsFailed    _ -> ()

    member x.Add =    add;
    member x.Remove = remove;