namespace Nikeza.Mobile.UILogic.Registration

open System.Windows.Input
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Adapter
open Nikeza.Mobile.Profile
open Nikeza.Mobile.Profile.Events
open Nikeza.Mobile.Profile.Commands.Registration

module Updates =
    let statusOf formValidated events = 
        events |> List.exists formValidated

type SideEffectFunctions = {
    Submit : Try.SubmitFn
}

type Responders = {
    ForRegistrationSubmission : (RegistrationSubmissionEvent -> unit) list
}

type Dependencies = {
    SideEffectFunctions : SideEffectFunctions
    EventResponders     : Responders
}

type ViewModel(dependencies) as x =

    inherit ViewModelBase()

    let sideEffects = dependencies.SideEffectFunctions
    let responders =  dependencies.EventResponders

    let mutable validatedForm = None

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

        let handle event = 
            responders.ForRegistrationSubmission |> List.iter(fun handle -> handle event)

        let broadcast (events:RegistrationSubmissionEvent list) = 
            events |> List.iter handle

        validatedForm |> function 
                         | Some form -> 
                                form |> Command.Execute 
                                     |> In.SubmitRegistration.workflow sideEffects.Submit
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

    member val Events = [] with get,set
    
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