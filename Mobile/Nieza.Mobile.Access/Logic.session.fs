namespace Are

open Nikeza.Mobile.Access.Commands.Session
open Nikeza.Mobile.Access.Events
open Nikeza.DataTransfer
open Nikeza.Common

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