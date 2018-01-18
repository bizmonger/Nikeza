namespace Nikeza.Mobile.Portal.DataSources

open System
open System.Collections.ObjectModel
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Profile.Events

type ViewModel(platformsFn) as x =

    inherit ViewModelBase()

    let mutable platforms = ObservableCollection<string>()
    let mutable platform =  ""
    let mutable accessId =  ""
    let mutable sources =   ObservableCollection<string>()
    let mutable validated = false

    let canAdd() =
        x.Validated <-
            not <| String.IsNullOrEmpty x.Platform &&
            not <| String.IsNullOrEmpty x.AccessId

        x.Validated

    let add =    DelegateCommand( (fun _ -> () (*todo...*)) , fun _ -> true )
    let remove = DelegateCommand( (fun _ -> () (*todo...*)) , fun _ -> true )

    member x.Platform
             with get() =      platform
             and  set(value) = platform <- value
                               base.NotifyPropertyChanged(<@ x.Platform @>)

    member x.AccessId
             with get() =      accessId
             and  set(value) = accessId <- value
                               base.NotifyPropertyChanged(<@ x.AccessId @>)
                               canAdd() |> ignore

    member x.Sources
             with get() =      sources
             and  set(value) = sources <- value
                               base.NotifyPropertyChanged(<@ x.Sources @>)

    member x.Platforms
             with get() =      platforms
             and  set(value) = platforms <- value
                               base.NotifyPropertyChanged(<@ x.Platforms @>)

    member x.Validated
             with get() =      validated
             and  set(value) = validated <- value
                               base.NotifyPropertyChanged(<@ x.Validated @>)

    member x.Init() =
    
             platformsFn()
              |> function
                 | PlatformsSucceeded p -> x.Platforms <- ObservableCollection<string>(p)
                 | PlatformsFailed    _ -> ()

    member x.Add =    add;
    member x.Remove = remove;