namespace Nikeza.Mobile.UILogic

open System.ComponentModel
open Microsoft.FSharp.Quotations.Patterns

type ViewModelBase () =

    let propertyChanged = 
        Event<PropertyChangedEventHandler,PropertyChangedEventArgs>()
        
    let getPropertyName = function 
        | PropertyGet(_,pi,_) -> pi.Name
        | _ -> invalidOp "Expecting property getter expression"
        
    interface INotifyPropertyChanged with

        [<CLIEvent>]
        member this.PropertyChanged = propertyChanged.Publish
        
    member private this.NotifyPropertyChanged propertyName = 
        propertyChanged.Trigger(this,PropertyChangedEventArgs(propertyName))

    member this.NotifyPropertyChanged quotation = 
        quotation |> getPropertyName |> this.NotifyPropertyChanged