module In

open Nikeza.Mobile.Profile.Commands
open Nikeza.Mobile.Profile.Commands.Registration
open Nikeza.Mobile.Profile.Events
open Logic

module SubmitRegistration =
    type private SubmitWorkflow = Try.SubmitFn -> Command -> RegistrationSubmissionEvent list

    let workflow : SubmitWorkflow =
        fun submitFn command -> command |> function
            Command.Execute form -> 
                            form |> submitFn
                                 |> ResultOf.Submit.Executed
                                 |> Are.Registration.Submission.events

module ValidateRegistration =
    type private ValidateWorkflow = Validate -> RegistrationValidationEvent list

    let workflow : ValidateWorkflow =
        fun command -> command |> function
            Validate.Execute form -> 
                             form |> Registration.validate
                                  |> ResultOf.Validation.Executed
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

module Edit =
    type private EditWorkflow = EditCommand -> ProfileEvent list

    let workflow : EditWorkflow = 
        fun command -> command |> function
        | EditCommand.Validate profile -> 
                               profile |> Edit.validate 
                                       |> ResultOf.Editor.Validate
                                       |> Are.Edit.events
        | EditCommand.Save     profile -> 
                               profile |> Try.save
                                       |> ResultOf.Editor.Save
                                       |> Are.Edit.events