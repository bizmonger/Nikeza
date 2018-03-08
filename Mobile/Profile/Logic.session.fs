module Are.Session

open Nikeza.Mobile.Profile.Commands
open Nikeza.Mobile.Profile.Events

type private HandleLogin = ResultOf.Session -> SessionEvent list

    let events : HandleLogin = function 
        | ResultOf.Session.Login result -> 
                                 result |> function
                                           | Ok    info -> [LoggedIn    info]
                                           | Error info -> [LoginFailed info]
        | ResultOf.Session.Logout result -> 
                                  result |> function 
                                            | Ok    _ -> [LoggedOut]
                                            | Error _ -> [LogoutFailed]