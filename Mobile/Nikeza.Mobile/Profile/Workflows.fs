module Workflows

open Commands
open Events
open WorkflowDetails

let handleRegistration = function
    | ValidateRegistration form -> validateRegistration form
    | _ -> []

let handleSession = function
    | HandleLogin  response -> response |> function
                                           | Ok    provider    -> [LoggedIn    provider]
                                           | Error credentials -> [LoginFailed credentials]

    | HandleLogout response -> response |> function
                                           | Ok _              -> [LoggedOut]
                                           | Error credentials -> [LogoutFailed]
    | _ -> []

let handleEdit = function
    | ValidateEdit profile  -> validateEdit profile 
    | HandleSave   response -> response |> function
                                           | Ok    profile -> [ProfileSaved      profile]
                                           | Error profile -> [ProfileSaveFailed profile]
    | _ -> []