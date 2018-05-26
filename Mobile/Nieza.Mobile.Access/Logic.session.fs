namespace Are

open Nikeza.Access.Specification.Commands.Session
open Nikeza.Access.Specification.Events
open Nikeza.Common
open Nikeza.DataTransfer

module Login =

    type private HandleLogin = ResultOf.Login -> LoginEvent nonempty

    let events : HandleLogin = function 
        ResultOf.Login result -> 
                       result |> function
                                 | Ok    info -> 
                                         info |> function
                                                 | Some p -> { Head= LoggedIn p; Tail=[] }

                                                 | None   -> let credentials = { Credentials.Email=""; Credentials.Password="" }
                                                             { Head= FailedToAuthenticate credentials; Tail=[] }
                                                 
                                 | Error info -> { Head= FailedToConnect info; Tail=[] }
            
module Logout =
    type private HandleLogout = ResultOf.Logout -> LogoutEvent nonempty

    let events : HandleLogout = function 
        ResultOf.Logout result -> 
                        result |> function 
                                  | Ok    p -> { Head= LoggedOut    p; Tail= [] }
                                  | Error p -> { Head= LogoutFailed p; Tail= [] }