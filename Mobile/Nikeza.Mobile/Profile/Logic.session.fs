module internal Logic.Session

open Commands
open Events

type HandleLogin = ResultOf.Session -> SessionEvent list

let handle : HandleLogin = 
    fun result -> 
        result |> function 
                  | ResultOf.Session.Login result -> result |> function
                                                               | Ok    info -> [LoggedIn    info]
                                                               | Error info -> [LoginFailed info]

                  | ResultOf.Session.Logout result -> result |> function 
                                                                | Ok    _ -> [LoggedOut]
                                                                | Error _ -> [LogoutFailed] 