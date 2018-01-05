namespace Nikeza.Mobile.UILogic

open System.Windows.Input
open System

type DelegateCommand (action:(obj -> unit), canExecute:(obj -> bool)) =
    let event = new DelegateEvent<EventHandler>()
    interface ICommand with
        [<CLIEvent>]
        member x.CanExecuteChanged = event.Publish
        member x.CanExecute arg = canExecute(arg)
        member x.Execute arg = action(arg)