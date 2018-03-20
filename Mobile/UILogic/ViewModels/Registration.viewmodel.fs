namespace Nikeza.Mobile.UILogic.Registration

open System.Windows.Input
open Nikeza.Common
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Adapter
open Nikeza.Mobile.Access
open Nikeza.Mobile.Access.Events
open Nikeza.Mobile.Access.Commands.Registration

module Updates =
    let statusOf formValidated events = 
        events |> List.exists formValidated

type Implementation = {
    Submit : Try.SubmitFn
}

type SideEffects = {
    ForRegistrationSubmission : (RegistrationSubmissionEvent -> unit) list
}

type Dependencies = {
    Implementation : Implementation
    SideEffects    : SideEffects
}

type ViewModel(dependencies) as x =

    inherit ViewModelBase()

    let implementation = dependencies.Implementation
    let sideEffects =    dependencies.SideEffects

    let mutable validatedForm = None

    let validate() =
        let isValidated = function
            | FormValidated form -> validatedForm <- Some form; true
            | _ -> false

        { Adapter.Form.Email=    x.Email
          Adapter.Form.Password= x.Password
          Adapter.Form.Confirm=  x.Confirm
        } 
          |> ofUnvalidated
          |> Validate.Execute 
          |> In.ValidateRegistration.workflow
          |> Updates.statusOf isValidated
               
    let submit() =

        let broadcast events =
            events |> List.iter (fun event -> sideEffects.ForRegistrationSubmission |> handle event)

        validatedForm |> function 
                         | Some form -> 
                                form |> Command.Execute 
                                     |> In.SubmitRegistration.workflow implementation.Submit
                                     |> broadcast
                                     
                         | None -> ()

    let validateCommand = DelegateCommand( (fun _ -> x.IsValidated <- validate()) , fun _ -> true)

    let submitCommand =   DelegateCommand( (fun _ -> submit() ),
                                            fun _ -> x.IsValidated <- validate(); true )

    let emailPlaceholder =    "enter email address"
    let passwordPlaceholder = "password"
    let confirmPlaceholder =  "confirm"

    let mutable email =    emailPlaceholder
    let mutable password = passwordPlaceholder
    let mutable confirm =  confirmPlaceholder
    let mutable isValidated = false

    member x.Validate = validateCommand :> ICommand
    member x.Submit =   submitCommand   :> ICommand
    
    member x.Email
             with get() =      email 
             and  set(value) = email <- value
                               base.NotifyPropertyChanged (<@ x.Email @>)

    member x.Password
             with get() =      password
             and  set(value) = password <- value
                               base.NotifyPropertyChanged (<@ x.Password @>)

    member x.Confirm
        with get() =           confirm
        and  set(value) =      confirm <- value
                               base.NotifyPropertyChanged (<@ x.Confirm @>)

    member x.IsValidated
             with get() =      isValidated
             and  set(value) = isValidated <- value
                               base.NotifyPropertyChanged (<@ x.IsValidated @>)

    member x.EmailPlaceholder =    emailPlaceholder
    member x.PasswordPlaceholder = emailPlaceholder
    member x.ConfirmPlaceholder =  emailPlaceholder