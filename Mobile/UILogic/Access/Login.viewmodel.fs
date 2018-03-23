namespace Nikeza.Mobile.UILogic.Login

open System.Windows.Input
open Nikeza.Common
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Access
open Nikeza.Mobile.Access.Commands
open Nikeza.Mobile.Access.Events

type SideEffects =  { 
    ForLoginAttempt : (LoginEvent -> unit) list 
}

type Implementation =  { 
    Login : LoginFn
}

type Dependencies = { 
    Implementation : Implementation
    SideEffects    : SideEffects
}

type ViewModel(dependencies) as x =

    inherit ViewModelBase()

    let notAuthenticatedHandler = function
        | FailedToAuthenticate _ -> x.AuthenticationFailed <- true
        | _ -> ()

    let initialSideEffects = dependencies.SideEffects

    let sideEffects =      { initialSideEffects with ForLoginAttempt= notAuthenticatedHandler::initialSideEffects.ForLoginAttempt }
    let implementation =     dependencies.Implementation

    let mutable email =    ""
    let mutable password = ""
    let mutable isValidated =          false
    let mutable authenticationFailed = false

    let broadcast (events:LoginEvent list) = 
        events |> List.iter (fun event -> sideEffects.ForLoginAttempt |> handle event)

    let validate() =
        email.Length    > 0 &&
        password.Length > 0

    let OnNext _ =

        if   x.IsValidated

        then Submit { Email=email; Password=password }
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

    member x.AuthenticationFailed
             with get() =      authenticationFailed
             and  set(value) = authenticationFailed <- value
                               base.NotifyPropertyChanged (<@ x.AuthenticationFailed @>)