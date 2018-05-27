namespace Are

open Nikeza.Access.Specification.Commands.Session
open Nikeza.Access.Specification.Events
open Nikeza.Access.Specification.Session
open Nikeza.Common
open Nikeza.DataTransfer

module internal Login =

    let events : HandleLogin = function 
        ResultOf.Login result -> 
                       result |> function
                                 | Ok    info -> 
                                         info |> function
                                                 | Some p -> { Head= LoggedIn p; Tail=[] }

                                                 | None   -> let credentials = { Credentials.Email=""; Credentials.Password="" }
                                                             { Head= FailedToAuthenticate credentials; Tail=[] }
                                                 
                                 | Error info -> { Head= FailedToConnect info; Tail=[] }
            
module internal Logout =

    let events : HandleLogout = function 
        ResultOf.Logout result -> 
                        result |> function 
                                  | Ok    p -> { Head= LoggedOut    p; Tail= [] }
                                  | Error p -> { Head= LogoutFailed p; Tail= [] }