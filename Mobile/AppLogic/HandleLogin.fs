namespace Nikeza.Mobile.AppLogic

open Nikeza.Mobile.UI
open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Login
open Nikeza.Mobile.AppLogic.Navigation
open Design.Access

module LoginEvents =

    open Nikeza.Mobile.Access.Events
    
    let appendNavigation : Login.SideEffects =

        fun app sideEffects ->

            let handle = function
                | LoggedIn    provider    -> app |> navigate (new PortalPage()) provider
                | LoginFailed credentials -> app |> navigate (new ErrorPage())  credentials.Email

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