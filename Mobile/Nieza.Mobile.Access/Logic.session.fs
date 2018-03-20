namespace Are

open Nikeza.Mobile.Access.Commands.Session
open Nikeza.Mobile.Access.Events

module Login =
    type private HandleLogin = ResultOf.Login -> LoginEvent list

    let events : HandleLogin = function 
        ResultOf.Login result -> 
                       result |> function
                                   | Ok    info -> [LoggedIn    info]
                                   | Error info -> [LoginFailed info]
            
module Logout =
    type private HandleLogout = ResultOf.Logout -> LogoutEvent list

    let events : HandleLogout = function 
        ResultOf.Logout result -> 
                        result |> function 
                                  | Ok    p -> [LoggedOut    p]
                                  | Error p -> [LogoutFailed p]