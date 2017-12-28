module Workflows

open Commands
open IO
open Logic
open Events

type RegistrationWorkflow = Command -> RegistrationEvent list
type SessionWorkflow =      Command -> SessionEvent      list
type ProfileWorkflow =      Command -> ProfileEvent      list

let handleRegistration : RegistrationWorkflow =
    fun command -> command |> function
    | Command.ValidateRegistration form -> form |> validateRegistration
                                                |> handleRegistration

    | Command.SubmitRegistration   form -> form |> trySubmit
                                                |> handleRegistration
    | _ -> []

//let handleSession : SessionWorkflow = 
//    fun command -> command |> function
//    | HandleLogin  response -> response |> function
//                                           | Ok    provider    -> [LoggedIn    provider]
//                                           | Error credentials -> [LoginFailed credentials]

//    | HandleLogout response -> response |> function
//                                           | Ok _    -> [LoggedOut]
//                                           | Error _ -> [LogoutFailed]
//    | _ -> []

//let handleEdit : ProfileWorkflow = 
//    fun command -> command |> function
//    | ValidateEdit profile  -> validateEdit profile 
//    | HandleSave   response -> response |> function
//                                           | Ok    profile -> [ProfileSaved      profile]
//                                           | Error profile -> [ProfileSaveFailed profile]
//    | _ -> []