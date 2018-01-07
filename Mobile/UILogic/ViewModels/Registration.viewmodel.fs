namespace Nikeza.Mobile.UILogic.Registration

open System.Windows.Input
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Publisher
open Nikeza.Mobile.UILogic.Adapter
open Nikeza.Mobile.Profile
open Nikeza.Mobile.Profile.Events
open Nikeza.Mobile.Profile.Commands.Registration

module Updates =
    let statusOf formValidated events = 
        events |> List.exists formValidated

type ViewModel(submitFn:Try.SubmitFn) as x =

    inherit ViewModelBase()

    let mutable validatedForm = None

    let eventOccurred = new Event<_>()

    let validate() =
        let isValidated = function
            | FormValidated form -> validatedForm <- Some form; true
            | _ -> false

        { Form.Email=    x.Email
          Form.Password= x.Password
          Form.Confirm=  x.Confirm
        } 
          |> ofUnvalidated
          |> Validate.Execute 
          |> In.ValidateRegistration.workflow
          |> Updates.statusOf isValidated
               
    let submit() =
        validatedForm |> function 
                         | Some form -> 
                                form |> Command.Execute 
                                     |> In.SubmitRegistration.workflow submitFn
                                     |> publishEvents eventOccurred
                         | None -> ()

    let validateCommand = DelegateCommand( (fun _ -> x.IsValidated <- validate()) , fun _ -> true)

    let submitCommand =   DelegateCommand( (fun _ -> submit() ),
                                            fun _ -> x.IsValidated <- validate(); true )

    let mutable email =    "<enter email address>"
    let mutable password = ""
    let mutable confirm =  ""
    let mutable isValidated = false

    member x.Validate = validateCommand :> ICommand
    member x.Submit =   submitCommand   :> ICommand

    [<CLIEvent>]
    member x.PageRequested = eventOccurred.Publish

    member x.Email
             with get() =      email 
             and  set(value) = email <- value
                               base.NotifyPropertyChanged (<@ x.Email @>)

    member x.Password
             with get() =      password
             and  set(value) = password <- value

    member x.Confirm
        with get() =           confirm
        and  set(value) =      confirm <- value

    member x.IsValidated
             with get() =      isValidated
             and  set(value) = isValidated <- value
                               base.NotifyPropertyChanged (<@ x.IsValidated @>)