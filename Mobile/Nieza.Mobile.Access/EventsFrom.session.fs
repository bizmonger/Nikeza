namespace Nikeza.Mobile.Access

open Nikeza.Access.Specification.Events
open Nikeza.Access.Specification.Session
open Nikeza.Common
open Nikeza.DataTransfer

module AttemptLogin =

    let toEvents : HandleLoginResult = function 

        result -> 
        result |> function
                  | Ok    info -> 
                          info |> function
                                  | Some p -> { Head= LoggedIn p; Tail=[] }

                                  | None   -> let credentials = { Credentials.Email=""; Credentials.Password="" }
                                              { Head= FailedToAuthenticate credentials; Tail=[] }
                                  
                  | Error info -> { Head= FailedToConnect info; Tail=[] }
            
module AttemptLogout =

    let toEvents : HandleLogoutResult = function 
        result -> 
        result |> function 
                  | Ok    p -> { Head= LoggedOut    p; Tail= [] }
                  | Error p -> { Head= LogoutFailed p; Tail= [] }