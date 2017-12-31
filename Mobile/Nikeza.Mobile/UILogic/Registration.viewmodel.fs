namespace Nikeza.Mobile.UILogic.Registration

open System.Windows.Input
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Profile.Commands.Registration
open Nikeza.Mobile.Profile.Events
open Adapter

type UIForm = Registration.Types.Form
type DomainForm = Nikeza.Mobile.Profile.Registration.Form
type events<'a> = List<'a>

module That =
    let generatesPage events =
        let eventToPage = function
        | RegistrationSucceeded _ -> () // displayPortal()
        | RegistrationFailed    _ -> () // displayError()

        events |> List.iter eventToPage

module Updates =
    let statusOf formValidated events = 
        events |> List.exists formValidated

type ViewModel() as x =

    let mutable validatedForm = None

    let validate() =
        let isValidated = function
            | FormValidated form -> validatedForm <- Some form; true
            | _ -> false

        { UIForm.Email=    x.Email
          UIForm.Password= x.Password
          UIForm.Confirm=  x.Confirm
        } 
        |> ofUnvalidated
        |> Validate.Execute 
        |> In.ValidateRegistration.workflow
        |> Updates.statusOf isValidated
               
    let submit() =
        validatedForm |> function 
                         | Some form ->
                                form |> Submit.Execute 
                                     |> In.SubmitRegistration.workflow
                                     |> That.generatesPage
                                     
                         | None -> ()

    let validateCommand = DelegateCommand( (fun _ -> x.IsValidated <- validate()) , fun _ -> true)

    let submitCommand =   DelegateCommand( (fun _ -> ()), 
                                            fun _ -> x.IsValidated <- validate(); x.IsValidated )
    let mutable email =    ""
    let mutable password = ""
    let mutable confirm =  ""
    let mutable isValidated = false

    member x.Validate = validateCommand :> ICommand
    member x.Submit =   submitCommand   :> ICommand

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