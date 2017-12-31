module Execute

open Commands
open Nikeza.Mobile.Profile.Events
open Logic

module Registration =
    type private RegistrationWorkflow = RegistrationCommand -> RegistrationEvent list

    let workflow : RegistrationWorkflow =
        fun command -> command |> function
        | RegistrationCommand.Validate form -> 
                                       form |> Registration.validate
                                            |> ResultOf.Registration.Validate
                                            |> Are.Registration.events
        | RegistrationCommand.Submit   form -> 
                                       form |> Try.submit
                                            |> ResultOf.Registration.Submit
                                            |> Are.Registration.events

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