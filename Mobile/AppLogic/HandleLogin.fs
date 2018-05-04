namespace Nikeza.Mobile.AppLogic

open Nikeza.Common
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Login
open Nikeza.Mobile.Access.Events
open Navigation
open PageFactory
open Design.Access

module LoginEvents =
    
    let appendNavigation : Login.SideEffects =

        fun app sideEffects ->

            let handle = function
                | LoggedIn        provider    -> app |> navigate (portalPage <| provider.Profile) provider
                | FailedToConnect credentials -> app |> navigate errorPage credentials.Email

                | FailedToAuthenticate _ -> ()

            let forLoginAttempt = sideEffects.ForLoginAttempt
            let handlers = { Head= forLoginAttempt.Head
                             Tail= handle::forLoginAttempt.Tail 
                           }

            { sideEffects with ForLoginAttempt= handlers }

module Login =

    open LoginEvents
    open Xamarin.Forms
    open System.Diagnostics

    let dependencies =

        let log = function
            | LoggedIn             user        -> Debug.WriteLine(sprintf "Login successful:\n %A" user)
            | FailedToConnect      credentials -> Debug.WriteLine(sprintf "Error: Unable to connect to server:\n %A" credentials)
            | FailedToAuthenticate credentials -> Debug.WriteLine(sprintf "Warning: Unable to authenticate user:\n %A" credentials)

        let handlers =       { Head=log; Tail=[] }
        let sideEffects =    { ForLoginAttempt= handlers } |> appendNavigation Application.Current
        let implementation = { Login= TestAPI.mockLogin }
    
        { SideEffects=    sideEffects; 
          Implementation= implementation 
        }