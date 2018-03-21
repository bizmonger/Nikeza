namespace Nikeza.Mobile.AppLogic

open Nikeza.Mobile.UILogic
open Nikeza.Mobile.UILogic.Login
open Design

module LoginEvents =

    open System.Diagnostics
    open Nikeza.Mobile.Access.Events
    
    let appendNavigation : ``Side effects from login`` =

        fun app sideEffects ->

            let handle = function
                | LoggedIn    provider    -> //app.MainPage = new PortalPage()
                                             Debug.WriteLine(sprintf "\nRequest: Navigate to Portal\n %A" provider)
                                             
                | LoginFailed credentials -> Debug.WriteLine(sprintf "\nLogin failed\n %A" credentials.Email)

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