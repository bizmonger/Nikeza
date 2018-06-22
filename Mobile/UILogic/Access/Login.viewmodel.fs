namespace Nikeza.Mobile.UILogic.Login

open System.Windows.Input
open Nikeza
open Nikeza.Common
open Nikeza.Access.Specification.Events
open Nikeza.Access.Specification.Login
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Access.AttemptLogin
open DataTransfer

type ViewModel(dependencies) as x =

    inherit ViewModelBase()

    let onAuthenticationFailed = function
        | FailedToAuthenticate _ -> x.AuthenticationFailed <- true
        | _ -> ()

    let sideEffects' =    dependencies.SideEffects
    let forLoginAttempt = sideEffects'.ForLoginAttempt
    let updates =       { forLoginAttempt with Tail=onAuthenticationFailed::forLoginAttempt.Tail }
    let sideEffects =   { sideEffects' with ForLoginAttempt= updates }
    let tryLogin =        dependencies.Attempt.Login

    let mutable email =    ""
    let mutable password = ""
    let mutable isValidated =          false
    let mutable authenticationFailed = false

    let broadcast events = 
        events.Head::events.Tail 
         |> List.iter (fun event -> sideEffects.ForLoginAttempt |> handle event)

    let validate() =
        email    .Length > 0 &&
        password .Length > 0

    let OnNext _ =

        if   x.IsValidated

        then { Email=email; Password=password }
                |> tryLogin
                |> toEvents
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