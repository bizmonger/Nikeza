namespace Nikeza.Mobile.UILogic.Login

open System.Windows.Input
open Nikeza.Common
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Profile
open Nikeza.Mobile.Profile.Commands
open Nikeza.Mobile.Profile.Events

type SideEffects =  { ForLoginAttempt : (SessionEvent -> unit) list }
type Dependencies = { SideEffects : SideEffects }

type ViewModel(dependencies) as x =

    inherit ViewModelBase()

    let sideEffects = dependencies.SideEffects

    let mutable email =    ""
    let mutable password = ""
    let mutable isValidated = false

    let broadcast events = 
        events |> List.iter (fun event -> sideEffects.ForLoginAttempt |> handle event)

    let validate() =
        email.Length    > 0 &&
        password.Length > 0

    let OnNext _ =
        if   x.IsValidated
        then In.Session.workflow <| Login { Email=email; Password=password }
             |> broadcast
        else ()

    let nextCommand = DelegateCommand( OnNext , fun _ -> true)

    member x.Next = nextCommand :> ICommand

    member x.Email
             with get() =      email 
             and  set(value) = email <- value
                               x.IsValidated <- validate()
                               base.NotifyPropertyChanged (<@ x.Email @>)

    member x.Password
             with get() =      password
             and  set(value) = password <- value
                               x.IsValidated <- validate()
                               base.NotifyPropertyChanged (<@ x.Password @>)

    member x.IsValidated
             with get() =      isValidated
             and  set(value) = isValidated <- value
                               base.NotifyPropertyChanged (<@ x.IsValidated @>)