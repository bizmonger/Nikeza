module Workflows

open Commands
open IO
open Logic.Registration
open Events
open Logic

type RegistrationWorkflow = Command -> RegistrationEvent list
type SessionWorkflow =      Command -> SessionEvent      list
type ProfileWorkflow =      Command -> ProfileEvent      list

let handleRegistration : RegistrationWorkflow =
    fun command -> command |> function
    | Command.ValidateRegistration form -> form |> validate
                                                |> ResultOf.ValidateEdit
                                                |> Registration.handle

    | Command.SubmitRegistration   form -> form |> trySubmit
                                                |> ResultOf.SubmitRegistration
                                                |> Registration.handle
    | _ -> []

let handleSession : SessionWorkflow = 
    fun command -> command |> function
    | Command.HandleLogin  credentials -> credentials |> tryLogin
                                                      |> ResultOf.Login
                                                      |> Session.handle

    | Command.HandleLogout -> tryLogout()
                               |> ResultOf.Logout
                               |> Session.handle
    | _ -> []

//let handleEdit : ProfileWorkflow = 
//    fun command -> command |> function
//    | ValidateEdit profile  -> validateEdit profile 
//    | HandleSave   response -> response |> function
//                                           | Ok    profile -> [ProfileSaved      profile]
//                                           | Error profile -> [ProfileSaveFailed profile]
//    | _ -> []