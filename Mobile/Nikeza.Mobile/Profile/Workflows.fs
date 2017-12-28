module Workflows

open Commands
open IO
open Logic.Registration
open Events
open Logic

type RegistrationWorkflow = RegistrationCommand -> RegistrationEvent list
type SessionWorkflow =      SessionCommand      -> SessionEvent      list
type EditWorkflow =         EditCommand         -> ProfileEvent      list

let handleRegistration : RegistrationWorkflow =
    fun command -> command |> function
    | RegistrationCommand.Validate form -> form |> validate
                                                |> ResultOf.Registration.Validate
                                                |> Registration.handle

    | RegistrationCommand.Submit   form -> form |> trySubmit
                                                |> ResultOf.Registration.Submit
                                                |> Registration.handle

let handleSession : SessionWorkflow = 
    fun command -> command |> function
    | SessionCommand.Login credentials -> credentials |> tryLogin
                                                      |> ResultOf.Login
                                                      |> Session.handle
    | SessionCommand.Logout -> tryLogout()
                                |> ResultOf.Logout
                                |> Session.handle

let handleEdit : EditWorkflow = 
    fun command -> command |> function
    | EditCommand.Validate profile -> profile |> Edit.validate 
                                              |> ResultOf.Editor.Validate
                                              |> Edit.handle

    | EditCommand.Save     profile -> profile |> IO.trySave
                                              |> ResultOf.Editor.Save
                                              |> Edit.handle