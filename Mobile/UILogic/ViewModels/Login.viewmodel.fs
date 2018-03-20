namespace Nikeza.Mobile.UILogic.Login

open System.Windows.Input
open Nikeza.Common
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Access
open Nikeza.Mobile.Access.Commands
open Nikeza.Mobile.Access.Events
open Nikeza.Mobile.Access.Try

type SideEffects =  { 
    ForLoginAttempt : (LoginEvent -> unit) list 
}

type Implementation =  { 
    Login : LoginFn
}

type Dependencies = { 
    SideEffects    : SideEffects
    Implementation : Implementation
}

type ViewModel(dependencies) as x =

    inherit ViewModelBase()

    let sideEffects =    dependencies.SideEffects
    let implementation = dependencies.Implementation

    let mutable email =    ""
    let mutable password = ""
    let mutable isValidated = false

    let broadcast (events:LoginEvent list) = 
        events |> List.iter (fun event -> sideEffects.ForLoginAttempt |> handle event)

    let validate() =
        email.Length    > 0 &&
        password.Length > 0

    let OnNext _ =

        if   x.IsValidated

        then Login { Email=email; Password=password }
             |> In.Login.workflow implementation.Login
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