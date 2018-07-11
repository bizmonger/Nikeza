namespace Nikeza.Mobile.AppLogic

open Xamarin.Forms
open Nikeza.Common
open Nikeza.Mobile.UILogic
open Nikeza.Access.Specification.Login
open Nikeza.Access.Specification.Events
open Navigation
open PageFactory


module Login =

    open System.Diagnostics

    let dependencies =

        let log' = function
            | LoggedIn             user        -> Debug.WriteLine(sprintf "Login successful:\n %A" user)
            | FailedToConnect      credentials -> Debug.WriteLine(sprintf "Error:   Unable to connect to server:\n %A" credentials)
            | FailedToAuthenticate credentials -> Debug.WriteLine(sprintf "Warning: Unable to authenticate user:\n %A" credentials)

        let navigate' = function
            | LoggedIn        provider    -> Application.Current |> navigate (portalPage provider.Profile) provider
            | FailedToConnect credentials -> Application.Current |> navigate errorPage credentials.Email

            | FailedToAuthenticate _ -> ()

        let handlers =    { Head=log'; Tail=[navigate'] }
        let sideEffects = { ForLoginAttempt= handlers }
        let attempt =     { Login= TestAPI.mockLogin }
    
        { SideEffects= sideEffects; 
          Attempt=     attempt 
        }