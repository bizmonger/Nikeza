namespace Nikeza.Mobile.UILogic.Login

open System.Windows.Input
open Nikeza.Mobile.UILogic

type ViewModel() as x =

    inherit ViewModelBase()

    let mutable email =    ""
    let mutable password = ""
    let mutable isValidated = false

    let validate() =
        email.Length    > 0 &&
        password.Length > 0

    let nextCommand = DelegateCommand( (fun _ -> x.IsValidated <- validate()) , fun _ -> true)

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