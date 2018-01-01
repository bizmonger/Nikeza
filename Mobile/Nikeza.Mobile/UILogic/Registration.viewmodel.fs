namespace Nikeza.Mobile.UILogic.Registration

open System.Windows.Input
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Profile.Commands.Registration
open Nikeza.Mobile.Profile.Events
open Adapter
open In

type Form = Registration.Types.Form
type DomainForm = Nikeza.Mobile.Profile.Registration.Form
type events<'a> = List<'a>

module Updates =
    let statusOf formValidated events = 
        events |> List.exists formValidated

type ViewModel(submitFn:Try.SubmitRegistration) as x =

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
        let publishEvents events =
            events |> List.iter(fun event -> eventOccurred.Trigger(event))

        validatedForm |> function 
                         | Some form -> 
                                form |> Submit.Execute 
                                     |> In.SubmitRegistration.workflow submitFn
                                     |> publishEvents
                         | None -> ()

    let validateCommand = DelegateCommand( (fun _ -> x.IsValidated <- validate()) , fun _ -> true)

    let submitCommand =   DelegateCommand( (fun _ -> submit() |> ignore), 
                                            fun _ -> x.IsValidated <- validate(); x.IsValidated )

    let mutable email =    ""
    let mutable password = ""
    let mutable confirm =  ""
    let mutable isValidated = false

    member x.Validate = validateCommand :> ICommand
    member x.Submit =   submitCommand   :> ICommand

    [<CLIEvent>]
    member x.EventOccured = eventOccurred.Publish

    member x.Email
             with get() =      email 
             and  set(value) = email <- value

    member x.Password
             with get() =      password
             and  set(value) = password <- value

    member x.Confirm
        with get() =      confirm
        and  set(value) = confirm <- value

    member x.IsValidated
             with get() =      isValidated
             and  set(value) = isValidated <- value