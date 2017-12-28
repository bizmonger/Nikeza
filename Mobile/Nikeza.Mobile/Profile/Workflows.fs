module Workflows

open Commands
open IO
open Logic.Registration
open Events
open Logic

type RegistrationWorkflow = Command -> RegistrationEvent list
type SessionWorkflow =      Command -> SessionEvent      list
type EditWorkflow =          Command -> ProfileEvent      list

let handleRegistration : RegistrationWorkflow =
    fun command -> command |> function
    | Command.ValidateRegistration form -> form |> validate
                                                |> ResultOf.ValidateRegistration
                                                |> Registration.handle

    | Command.SubmitRegistration   form -> form |> trySubmit
                                                |> ResultOf.SubmitRegistration
                                                |> Registration.handle
    | _ -> []

let handleSession : SessionWorkflow = 
    fun command -> command |> function
    | Command.Login credentials -> credentials |> tryLogin
                                               |> ResultOf.Login
                                               |> Session.handle

    | Command.Logout -> tryLogout()
                         |> ResultOf.Logout
                         |> Session.handle
    | _ -> []

let handleEdit : EditWorkflow = 
    fun command -> command |> function
    | Command.ValidateEdit profile  -> profile |> Edit.validate 
                                               |> ResultOf.ValidateProfile
                                               |> Edit.handle

    | Command.Save         profile  -> profile |> IO.trySave
                                               |> ResultOf.Save
                                               |> Edit.handle
    | _ -> []