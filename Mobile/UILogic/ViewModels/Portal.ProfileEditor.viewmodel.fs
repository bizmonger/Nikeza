namespace Nikeza.Mobile.UILogic.Portal.ProfileEditor

open System
open System.Windows.Input
open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Publisher
open Nikeza.Mobile.Profile
open Nikeza.Mobile.Profile.Try
open Nikeza.Mobile.Profile.Query
open Nikeza.Mobile.Profile.Events
open Nikeza.Mobile.Profile.Commands.ProfileEditor
open System.Collections.ObjectModel

type ViewModel(user:Profile, saveFn:SaveFn, topicsFn:TopicsFn) as x =

    inherit ViewModelBase()

    let saveEvent =   new Event<_>()
    let topicsEvent = new Event<_>()

    let mutable firstNamePlaceholder = "<first name>"
    let mutable lastNamePlaceholder =  "<last name>"

    let mutable profile =     user
    let mutable firstName =   firstNamePlaceholder
    let mutable lastName =    lastNamePlaceholder
    let mutable email =       user.Email
    let mutable topics =      ObservableCollection<string>()
    let mutable isValidated = false

    let canSave() =
        let refreshState =
            profile <- { Id =       user.Id
                         FirstName= x.FirstName
                         LastName=  x.LastName
                         Email=     x.Email
                         Bio=       user.Bio
                         ImageUrl=  user.ImageUrl
                         Sources=   user.Sources
                       }
    
        if not <| String.IsNullOrWhiteSpace x.FirstName &&
                  String.IsNullOrWhiteSpace x.LastName  &&
                  String.IsNullOrWhiteSpace x.Email    

           then refreshState
                x.IsValidated <- true;  true

           else x.IsValidated <- false; false

    let save() = 
        { Profile= profile }
           |> SaveCommand.Execute 
           |> In.Editor.Save.workflow saveFn
           |> publishEvents saveEvent
                

    member x.Validate = DelegateCommand( (fun _ -> canSave() |> ignore) , 
                                          fun _ -> true )      :> ICommand

    member x.Save =     DelegateCommand( (fun _ -> save()) ,
                                          fun _ -> canSave() ) :> ICommand

    member x.FirstName
        with get() =       firstName
        and set(value) =   firstName <- value
                           base.NotifyPropertyChanged (<@ x.FirstName @>)

    member x.LastName
        with get() =       lastName
        and set(value) =   lastName  <- value
                           base.NotifyPropertyChanged (<@ x.LastName @>)

    member x.Email
        with get() =       email
        and set(value) =   email     <- value
                           base.NotifyPropertyChanged (<@ x.Email @>)
                                     
    member x.Topics                  
        with get() =       topics    
        and set(value) =   topics    <- value
                           base.NotifyPropertyChanged (<@ x.Topics @>)
                         
    member x.IsValidated
         with get() =      isValidated
         and  set(value) = isValidated <- value
                           base.NotifyPropertyChanged (<@ x.IsValidated @>)

    member x.Load() =
        topicsFn()
         |> function
            | Query.TopicsSucceeded v -> topics <- ObservableCollection(v |> Seq.map (fun topic -> topic.Name))
            | Query.TopicsFailed _    -> publishEvent topicsEvent Pages.Error

    member x.FirstNamePlaceholder
        with get() = firstNamePlaceholder

    member x.LastNamePlaceholder
        with get() = lastNamePlaceholder

    [<CLIEvent>]
    member x.SaveEvent =   saveEvent.Publish

    [<CLIEvent>]
    member x.TopicsEvent = topicsEvent.Publish