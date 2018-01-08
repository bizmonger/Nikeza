namespace Nikeza.Mobile.UILogic.Portal.ProfileEditor

open System
open System.Windows.Input
open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Publisher
open Nikeza.Mobile.Profile
open Nikeza.Mobile.Profile.Commands
open Nikeza.Mobile.Profile.Try

type ViewModel(user:Profile, saveFn:SaveFn) as x =

    inherit ViewModelBase()

    let eventOcurred = new Event<_>()

    let mutable defaultFirstName = "<first name>"
    let mutable defaultLastName =  "<last name>"

    let mutable profile =     user
    let mutable firstName =   defaultFirstName
    let mutable lastName =    defaultLastName
    let mutable email =       user.Email
    let mutable topics =      []
    let mutable isValidated = false

    let canSave() =
        let refreshState =
            profile <- { Id =       user.Id
                         FirstName= x.FirstName
                         LastName=  x.LastName
                         Email=     x.Email
                         Bio=       ""
                         ImageUrl=  ""
                         Sources=   []
                       }
    
        if not ( String.IsNullOrWhiteSpace (x.FirstName) &&
                                 String.IsNullOrWhiteSpace (x.LastName ) &&
                                 String.IsNullOrWhiteSpace (x.Email    ) )

           then refreshState
                x.IsValidated <- true;  true
           else x.IsValidated <- false; false

    let save() = 
        { Profile= profile }
           |> EditCommand.Save 
           |> In.Edit.workflow saveFn
           |> publishEvents eventOcurred
                

    member x.Validate = DelegateCommand( (fun _ -> canSave() |> ignore) , 
                                          fun _ -> true )      :> ICommand

    member x.Save =     DelegateCommand( (fun _ -> save()) ,
                                          fun _ -> canSave() ) :> ICommand

    member x.FirstName
        with get() =     firstName
        and set(value) = firstName <- value
                         base.NotifyPropertyChanged (<@ x.FirstName @>)

    member x.LastName
        with get() =     lastName
        and set(value) = lastName  <- value
                         base.NotifyPropertyChanged (<@ x.LastName @>)

    member x.Email
        with get() =     email
        and set(value) = email     <- value
                         base.NotifyPropertyChanged (<@ x.Email @>)
                                   
    member x.Topics                
        with get() =     topics    
        and set(value) = topics    <- value
                         base.NotifyPropertyChanged (<@ x.Topics @>)
                         
    member x.IsValidated
             with get() =      isValidated
             and  set(value) = isValidated <- value
                               base.NotifyPropertyChanged (<@ x.IsValidated @>)

    [<CLIEvent>]
    member x.EventOccurred = eventOcurred.Publish

    member x.FirstNameDefault
             with get() = defaultFirstName

    member x. LastNameDefault 
              with get() = defaultLastName