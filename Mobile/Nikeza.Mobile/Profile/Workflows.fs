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
                                        |> Registration.handle
    | RegistrationCommand.Submit   form -> 
                                   form |> IO.trySubmit
                                        |> ResultOf.Registration.Submit
                                        |> Registration.handle

let handleSession : SessionWorkflow = 
    fun command -> command |> function
    | SessionCommand.Login credentials -> 
                           credentials |> IO.tryLogin
                                       |> ResultOf.Login
                                       |> Session.handle

    | SessionCommand.Logout -> IO.tryLogout()
                                 |> ResultOf.Logout
                                 |> Session.handle

let handleEdit : EditWorkflow = 
    fun command -> command |> function
    | EditCommand.Validate profile -> 
                           profile |> Edit.validate 
                                   |> ResultOf.Editor.Validate
                                   |> Edit.handle
    | EditCommand.Save     profile -> 
                           profile |> IO.trySave
                                   |> ResultOf.Editor.Save
                                   |> Edit.handle