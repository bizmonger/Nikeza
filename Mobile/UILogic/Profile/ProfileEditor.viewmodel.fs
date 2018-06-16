namespace Nikeza.Mobile.UILogic.Portal.ProfileEditor

open System
open System.Windows.Input
open System.Collections.ObjectModel
open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Profile.Attempt
open Nikeza.Mobile.Profile.Queries
open Nikeza.Mobile.Profile.Commands.ProfileEditor
open Nikeza.Mobile.Profile.Commands.ProfileEditor.Save
open Nikeza.Portal.Specification.Events

type Attempt = {
    Save : SaveProfileFn
}

type Query = {
    Topics : TopicsFn
}

type SideEffects = {
    ForProfileSave       : (SaveProfileEvent -> unit) list
    ForQueryTopicsFailed : (Result<Topic list, string> -> unit) list
}

type Dependencies = {
    User        : Profile
    Query       : Query
    Attempt     : Attempt
    SideEffects : SideEffects
}

type ViewModel(dependencies) as x =

    inherit ViewModelBase()

    let query =          dependencies.Query
    let user =           dependencies.User
    let save =           dependencies.Attempt.Save
    let sideEffects =    dependencies.SideEffects
    
    let mutable firstNamePlaceholder = "first name"
    let mutable lastNamePlaceholder =  "last name"

    let mutable profile =        user
    let mutable firstName =      firstNamePlaceholder
    let mutable lastName =       lastNamePlaceholder
    let mutable email =          user.Email
    let mutable topics =         ObservableCollection<string>()
    let mutable featuredTopics = ObservableCollection<string>()
    let mutable topic:string =   null
    let mutable isValidated =    false

    let canSave() =
        let refreshState() =
        
            profile <- { Id =       user.Id
                         FirstName= x.FirstName
                         LastName=  x.LastName
                         Email=     x.Email
                         Bio=       user.Bio
                         ImageUrl=  user.ImageUrl
                         Sources=   user.Sources
                       }

        let containsDefault() = 
            x.FirstName = firstNamePlaceholder ||
            x.LastName  = lastNamePlaceholder

        if not <| containsDefault() &&
           not <| String.IsNullOrWhiteSpace x.FirstName &&
           not <| String.IsNullOrWhiteSpace x.LastName  &&
           not <| String.IsNullOrWhiteSpace x.Email

           then refreshState()
                x.IsValidated <- true;  true

           else x.IsValidated <- false; false

    let save() =
    
        let broadcast events = 
            events.Head::events.Tail |> List.iter (fun event -> sideEffects.ForProfileSave |> handle' event)
        
        Save { Profile= profile } 
         |> save
         |> ResultOf.Editor.Save
         |> Are.Editor.Save.events
         |> broadcast

    member x.FirstName
        with get() =       firstName
        and set(value) =   firstName <- value
                           base.NotifyPropertyChanged (<@ x.FirstName @>)
                           (x.Validate :> ICommand).Execute()

    member x.LastName
        with get() =       lastName
        and set(value) =   lastName  <- value
                           base.NotifyPropertyChanged (<@ x.LastName @>)
                           (x.Validate :> ICommand).Execute()

    member x.Email
        with get() =       email
        and set(value) =   email     <- value
                           base.NotifyPropertyChanged (<@ x.Email @>)
                           (x.Validate :> ICommand).Execute()
                           
    member x.Topics
        with get() =       topics    
        and  set(value) =  topics   <- value
                           base.NotifyPropertyChanged (<@ x.Topics @>)
                           (x.Validate :> ICommand).Execute()

    member x.FeaturedTopics
        with get() =       featuredTopics    
        and  set(value) =  featuredTopics <- value
                           base.NotifyPropertyChanged (<@ x.FeaturedTopics @>)
                           (x.Validate :> ICommand).Execute()

    member x.Topic
        with get() =       topic
        and  set(value) =  topic <- value
                           base.NotifyPropertyChanged (<@ x.Topic @>)
                           (x.Validate :> ICommand).Execute()

    member x.IsValidated
         with get() =      isValidated
         and  set(value) = isValidated <- value
                           base.NotifyPropertyChanged (<@ x.IsValidated @>)

    member x.Validate = DelegateCommand( (fun _ -> canSave() |> ignore) , 
                                          fun _ -> true )      :> ICommand

    member x.Save =     DelegateCommand( (fun _ -> save()) ,
                                          fun _ -> true ) :> ICommand

    member x.Add =      DelegateCommand( (fun _ -> x.FeaturedTopics.Add(x.Topic)) ,
                                          fun _ -> true ) :> ICommand

    member x.FirstNamePlaceholder
        with get() = firstNamePlaceholder

    member x.LastNamePlaceholder
        with get() = lastNamePlaceholder

    member x.Init() =

        let broadcast (events:Result<Topic list, string> list) = 
            events |> List.iter (fun event -> sideEffects.ForQueryTopicsFailed |> handle' event)
            
        query.Topics()
            |> function
               | Ok     v   -> topics <- ObservableCollection(v |> Seq.map (fun topic -> topic.Name))
               | Error  msg -> broadcast [Error msg]