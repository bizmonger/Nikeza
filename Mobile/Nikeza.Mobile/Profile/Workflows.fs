module Workflows

open Commands
open Events
open Logic

type private RegistrationWorkflow = RegistrationCommand -> RegistrationEvent list
type private SessionWorkflow =      SessionCommand      -> SessionEvent      list
type private EditWorkflow =         EditCommand         -> ProfileEvent      list

let handleRegistration : RegistrationWorkflow =
    fun command -> command |> function
    | RegistrationCommand.Validate form -> 
                                   form |> Registration.validate
                                        |> ResultOf.Registration.Validate
                                        |> Are.Registration.events
    | RegistrationCommand.Submit   form -> 
                                   form |> Try.submit
                                        |> ResultOf.Registration.Submit
                                        |> Are.Registration.events
let handleSession : SessionWorkflow = 
    fun command -> command |> function
    | SessionCommand.Login credentials -> 
                           credentials |> Try.login
                                       |> ResultOf.Login
                                       |> Are.Session.events

    | SessionCommand.Logout -> Try.logout()
                                   |> ResultOf.Logout
                                   |> Are.Session.events
let handleEdit : EditWorkflow = 
    fun command -> command |> function
    | EditCommand.Validate profile -> 
                           profile |> Edit.validate 
                                   |> ResultOf.Editor.Validate
                                   |> Are.Edit.events
    | EditCommand.Save     profile -> 
                           profile |> Try.save
                                   |> ResultOf.Editor.Save
                                   |> Are.Edit.events