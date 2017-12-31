namespace UILogic.Registration

open UILogic
open System.Windows.Input
open Nikeza.Mobile.Profile.Commands
open Nikeza.Mobile.Profile.Registration
open Nikeza.Mobile.Profile.Events

type UIForm = Registration.Types.Form
type DomainForm = Nikeza.Mobile.Profile.Registration.Form

type ViewModel() as x =

    let form = { 
        UIForm.Email=    x.Email
        UIForm.Password= x.Password
        UIForm.Confirm=  x.Confirm
    }

    let toDomainForm (form:UIForm) : DomainForm = { 
          Email =    Email    form.Email
          Password = Password form.Password
          Confirm =  Password form.Password
    }

    let formValidated = function
        | FormValidated _ -> true
        | _               -> false

    let validate = DelegateCommand( (fun _ -> ()) , fun _ -> true)

    let submit =   DelegateCommand( (fun _ -> ()), 
                                     fun _ -> { UnvalidatedForm.Form= form |> toDomainForm } 
                                                |> RegistrationCommand.Validate 
                                                |> Execute.Registration.workflow
                                                |> List.exists formValidated )
    member x.Email =    ""
    member x.Password = ""
    member x.Confirm =  ""

    member x.Validate = validate :> ICommand
    member x.Submit =   submit   :> ICommand