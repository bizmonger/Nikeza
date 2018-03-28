namespace Nikeza.Mobile.AppLogic

open Nikeza.DataTransfer
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
                | LoggedIn        provider    -> app |> navigate (portalPage <| idFrom provider) provider
                | FailedToConnect credentials -> app |> navigate errorPage credentials.Email

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