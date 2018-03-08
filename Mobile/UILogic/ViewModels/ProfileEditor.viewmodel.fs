namespace Nikeza.Mobile.UILogic.Portal.ProfileEditor

open System
open System.Windows.Input
open Nikeza.Common
open Nikeza.DataTransfer
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.Profile
open Nikeza.Mobile.Profile.Try
open Nikeza.Mobile.Profile.Query
open Nikeza.Mobile.Profile.Commands.ProfileEditor
open System.Collections.ObjectModel
open Nikeza.Mobile.Profile.Events
open Nikeza.Mobile.UILogic.Response

type Actions = {
    Save : SaveProfileFn
}

type Query = {
    Topics : TopicsFn
}

type Observers = {
    ForProfileSave    : (ProfileSaveEvent  -> unit) list
    ForTopicsFnFailed : (QueryTopicsFailed -> unit) list
}

type Dependencies = {
    User      : Profile
    Query     : Query
    Actions   : Actions
    Observers : Observers
}

type ViewModel(dependencies) as x =

    inherit ViewModelBase()

    let query =          dependencies.Query
    let user =           dependencies.User
    let implementation = dependencies.Actions
    let responders =     dependencies.Observers
    
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
    
        let broadcast (events) = 
            events |> List.iter (fun event -> responders.ForProfileSave|> handle event)
        
        { Profile= profile }
           |> SaveCommand.Execute 
           |> In.Editor.Save.workflow implementation.Save
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

        let broadcast (events:QueryTopicsFailed list) = 
            events |> List.iter (fun event -> responders.ForTopicsFnFailed |> handle event)
            
        query.Topics()
            |> function
            | Ok    v -> topics <- ObservableCollection(v |> Seq.map (fun topic -> topic.Name))
            | Error msg -> broadcast [QueryTopicsFailed msg]