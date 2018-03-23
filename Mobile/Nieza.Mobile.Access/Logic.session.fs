namespace Are

open Nikeza.Mobile.Access.Commands.Session
open Nikeza.Mobile.Access.Events
open Nikeza.DataTransfer

module Login =
    type private HandleLogin = ResultOf.Login -> LoginEvent list

    let events : HandleLogin = function 
        ResultOf.Login result -> 
                       result |> function
                                   | Ok    info -> info |> function
                                                   | Some p -> [LoggedIn p]

                                                   | None   -> let credentials = { Credentials.Email=""; Credentials.Password="" }
                                                               [FailedToAuthenticate credentials]
                                                   
                                   | Error info -> [FailedToConnect info]
            
module Logout =
    type private HandleLogout = ResultOf.Logout -> LogoutEvent list

    let events : HandleLogout = function 
        ResultOf.Logout result -> 
                        result |> function 
                                  | Ok    p -> [LoggedOut    p]
                                  | Error p -> [LogoutFailed p]