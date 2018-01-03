module Nikeza.Mobile.UILogic.Portal.ProfileEditor

open Nikeza.Mobile.UILogic
open System
open Nikeza.Mobile.Profile.Commands
open Nikeza.Mobile.UILogic.Publisher
open Nikeza.Mobile.Profile
open Nikeza.Common
open Nikeza.DataTransfer

type ViewModel() as x =

    let eventOcurred = new Event<_>()

    let mutable profile:ValidatedProfile option= None 

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
                           |> In.Edit.workflow
                           |> publish eventOcurred
                   | None -> ()

    member x.SaveCommand = DelegateCommand( (fun _ -> save()) , fun _ -> canSave() )

    member x.FirstName = ""
    member x.LastName =  ""
    member x.Email =     ""
    member x.Topics =    []