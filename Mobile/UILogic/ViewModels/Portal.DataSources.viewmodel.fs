namespace Nikeza.Mobile.Portal.DataSources

open System
open Nikeza.Mobile.UILogic

type ViewModel() as x =
    inherit ViewModelBase()

    let mutable platform = ""
    let mutable accessId = ""
    let mutable sources =  ""

    let canAdd() =
        not <| String.IsNullOrEmpty x.Platform &&
               String.IsNullOrEmpty x.AccessId

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

    member x.Add =    add;
    member x.Remove = remove;