namespace Nikeza.Mobile.Portal.DataSources

open System
open System.Collections.ObjectModel
open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Profile
open Nikeza.Mobile.Profile.Query
open Nikeza.Mobile.Profile.Commands.DataSources
open Nikeza.Mobile.Profile.Events

type Actions = {
    Save : Try.SaveSourcesFn
}

type Query = {
    Platforms : PlatformsFn
}

type Observers = {
    ForSaveSources : (SourcesSaveEvent -> unit) list
}

type Dependencies = {
    UserId    : ProfileId
    Query     : Query
    Actions   : Actions
    Observers : Observers
}

type ViewModel(dependencies:Dependencies) as x =

    inherit ViewModelBase()

    let userId =     dependencies.UserId
    let query =      dependencies.Query
    let actions =    dependencies.Actions
    let responders = dependencies.Observers

    let mutable platforms = ObservableCollection<string>()
    let mutable platform =  ""
    let mutable accessId =  ""
    let mutable sources =   ObservableCollection<DataSourceSubmit>()
    let mutable validated = false

    let canAdd() =
        x.Validated <-
            not <| String.IsNullOrEmpty x.Platform &&
            not <| String.IsNullOrEmpty x.AccessId

        x.Validated

    let createSource() = {
        ProfileId= userId |> string
        Platform= x.Platform
        AccessId= x.AccessId
    }

    let save() = 

        let broadcast events =
            events |> List.iter (fun event -> responders.ForSaveSources |> handle event)

        x.Sources
           |> List.ofSeq
           |> SaveCommand.Execute 
           |> In.DataSources.Save.workflow actions.Save
           |> broadcast
    
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
    
             query.Platforms()
              |> function
                 | Ok p    -> x.Platforms <- ObservableCollection<string>(p)
                 | Error _ -> ()

    member x.Add =    DelegateCommand( (fun _ -> x.Sources.Add(createSource())) , fun _ -> true )
    member x.Remove = DelegateCommand( (fun _ -> () (*todo...*)) , fun _ -> true )
    member x.Save =   DelegateCommand( (fun _ -> save()) , fun _ -> true )