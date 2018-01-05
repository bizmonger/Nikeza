module Nikeza.Mobile.UILogic.Portal.ProfileEditor

open System
open System.Windows.Input
open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Profile.Commands
open Nikeza.Mobile.UILogic.Publisher
open Nikeza.Mobile.Profile.Try
open Nikeza.Mobile.Profile

type ViewModel(saveFn:SaveFn) as x =

    let eventOcurred = new Event<_>()

    let mutable profile = None
    let mutable firstName = ""
    let mutable lastName =  ""
    let mutable email =     ""
    let mutable topics =    []

    let canSave() =
        let refreshState =
            profile <- Some { Profile = { Id = ""
                                          FirstName= x.FirstName
                                          LastName=  x.LastName
                                          Email=     x.Email
                                          Bio=       ""
                                          ImageUrl=  ""
                                          Sources=   []
                                        }
                            }
    
        if not ( String.IsNullOrWhiteSpace (x.FirstName) &&
                 String.IsNullOrWhiteSpace (x.LastName ) &&
                 String.IsNullOrWhiteSpace (x.Email    ) )

           then refreshState; true
           else false

    let save() = 
        profile |> function
                   | Some validated -> 
                          validated 
                           |> EditCommand.Save 
                           |> In.Edit.workflow saveFn
                           |> publishEvents eventOcurred
                   | None -> ()

    member x.ValidateCommand = DelegateCommand( (fun _ -> canSave() |> ignore) , 
                                                 fun _ -> true )      :> ICommand

    member x.SaveCommand =     DelegateCommand( (fun _ -> save()) ,
                                                 fun _ -> canSave() ) :> ICommand

    member x.FirstName
        with get() =     firstName
        and set(value) = firstName <- value

    member x.LastName
        with get() =     lastName
        and set(value) = lastName  <- value

    member x.Email
        with get() =     email
        and set(value) = email  <- value

    member x.Topics
        with get() =     topics
        and set(value) = topics <- value

    member x.EventOccurred = eventOcurred.Publish