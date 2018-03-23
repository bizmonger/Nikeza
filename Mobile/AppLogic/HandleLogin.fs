namespace Nikeza.Mobile.AppLogic

open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Login
open Nikeza.Mobile.AppLogic.Navigation
open Design.Access
open PageFactory

module LoginEvents =

    open Nikeza.Mobile.Access.Events
    
    let appendNavigation : Login.SideEffects =

        fun app sideEffects ->

            let handle = function
                | LoggedIn        provider    -> app |> navigate portalPage provider
                | FailedToConnect credentials -> app |> navigate errorPage  credentials.Email

                | FailedToAuthenticate _ -> ()

            let handlers = handle::sideEffects.ForLoginAttempt

            { sideEffects with SideEffects.ForLoginAttempt= handlers }

module Login =

    open LoginEvents
    open Xamarin.Forms

    let dependencies =

        let sideEffects =    { ForLoginAttempt= [] } |> appendNavigation Application.Current
        let implementation = { Login= TestAPI.mockLogin }
    
        { SideEffects=    sideEffects; 
          Implementation= implementation 
        }