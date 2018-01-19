module Nikeza.Mobile.Profile.In

open Nikeza.Mobile.Profile.Commands
open Nikeza.Mobile.Profile.Commands.Registration
open Nikeza.Mobile.Profile.Events
open Logic
open Try

module SubmitRegistration =
    open Submit

    type private SubmitWorkflow = SubmitFn -> Command -> RegistrationSubmissionEvent list

    let workflow : SubmitWorkflow =
        fun submitFn command -> command |> function
            Command.Execute form -> 
                            form |> submitFn
                                 |> ResultOf.Submit.Executed
                                 |> Are.Registration.Submission.events

module ValidateRegistration =
    open Registration.Validate

    type private ValidateWorkflow = Validate -> RegistrationValidationEvent list

    let workflow : ValidateWorkflow =
        fun command -> command |> function
            Validate.Execute form -> 
                             form |> Registration.validate
                                  |> ResultOf.Validate.Executed
                                  |> Are.Registration.Validation.events

module Session =
    type private SessionWorkflow = SessionCommand -> SessionEvent list

    let workflow : SessionWorkflow = 
        fun command -> command |> function
        | SessionCommand.Login credentials -> 
                               credentials |> Try.login
                                           |> ResultOf.Login
                                           |> Are.Session.events
        
        | SessionCommand.Logout -> Try.logout()
                                       |> ResultOf.Logout
                                       |> Are.Session.events

module Editor =

    module Validate =
        open Commands.ProfileEditor.Validate
        open Commands.ProfileEditor
        
        type private ValidateWorkflow = ValidateCommand -> ProfileValidateEvent list

        let workflow : ValidateWorkflow = 
            fun command -> command |> function
             ValidateCommand.Execute form -> 
                                     form |> Editor.validate 
                                          |> ResultOf.Editor.Validate
                                          |> Are.Editor.Validate.events

    module Save =
        open Commands.ProfileEditor.Save
        open Commands.ProfileEditor

        type private SaveWorkflow = SaveFn -> SaveCommand -> ProfileSaveEvent list

        let workflow : SaveWorkflow = 
            fun saveFn command -> command |> function
             SaveCommand.Execute profile -> 
                                 profile |> saveFn
                                         |> ResultOf.Editor.Save
                                         |> Are.Editor.Save.events

module DataSources =

    module Save =
        open Commands.DataSources
        open Commands.DataSources.Save

        type private SaveWorkflow = SourcesSaveFn -> SaveCommand -> SourcesSaveEvent list

        let workflow : SaveWorkflow = 
            fun savefn command -> command |> function
                SaveCommand.Execute sources -> 
                                    sources |> savefn
                                            |> ResultOf.Save.Execute
                                            |> Are.DataSources.Save.events    