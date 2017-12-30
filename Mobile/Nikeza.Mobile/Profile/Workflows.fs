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
                                        |> Handle.Registration.result
    | RegistrationCommand.Submit   form -> 
                                   form |> Try.submit
                                        |> ResultOf.Registration.Submit
                                        |> Handle.Registration.result
let handleSession : SessionWorkflow = 
    fun command -> command |> function
    | SessionCommand.Login credentials -> 
                           credentials |> Try.login
                                       |> ResultOf.Login
                                       |> Handle.Session.result

    | SessionCommand.Logout -> Try.logout()
                                |> ResultOf.Logout
                                |> Handle.Session.result
let handleEdit : EditWorkflow = 
    fun command -> command |> function
    | EditCommand.Validate profile -> 
                           profile |> Edit.validate 
                                   |> ResultOf.Editor.Validate
                                   |> Edit.Result.handle
    | EditCommand.Save     profile -> 
                           profile |> Try.save
                                   |> ResultOf.Editor.Save
                                   |> Edit.Result.handle