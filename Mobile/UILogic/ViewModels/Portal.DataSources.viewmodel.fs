namespace Nikeza.Mobile.Portal.DataSources

open System
open System.Collections.ObjectModel
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Publisher
open Nikeza.Mobile.Profile
open Nikeza.Mobile.Profile.Events
open Nikeza.Mobile.Profile.Commands.DataSources

type ViewModel(user:Profile, platformsFn, savefn) as x =

    inherit ViewModelBase()

    let mutable platforms = ObservableCollection<string>()
    let mutable platform =  ""
    let mutable accessId =  ""
    let mutable sources =   ObservableCollection<DataSourceSubmit>()
    let mutable validated = false

    let saveRequest = Event<_>()

    let canAdd() =
        x.Validated <-
            not <| String.IsNullOrEmpty x.Platform &&
            not <| String.IsNullOrEmpty x.AccessId

        x.Validated

    let createSource() = {
        ProfileId= user.Id
        Platform= x.Platform
        AccessId= x.AccessId
    }

    let save() = 
        x.Sources
           |> List.ofSeq
           |> SaveCommand.Execute 
           |> In.DataSources.Save.workflow savefn
           |> publishEvents saveRequest
    
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

    member x.Add =    DelegateCommand( (fun _ -> x.Sources.Add(createSource())) , fun _ -> true )
    member x.Remove = DelegateCommand( (fun _ -> () (*todo...*)) , fun _ -> true )
    member x.Save =   DelegateCommand( (fun _ -> save()) , fun _ -> true )

    [<CLIEvent>]
    member x.SaveRequest = saveRequest.Publish