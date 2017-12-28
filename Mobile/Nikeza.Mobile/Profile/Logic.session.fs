module internal Logic.Session

open Commands
open Events

type HandleLogin = ResultOf -> SessionEvent list

let handle : HandleLogin = 
    fun result -> 
        result |> function 
                  | Login result -> result |> function
                                              | Ok    info -> [LoggedIn    info]
                                              | Error info -> [LoginFailed info]

                  | Logout result -> result |> function 
                                               | Ok    _ -> [LoggedOut]
                                               | Error _ -> [LogoutFailed] 
                  | _ -> []