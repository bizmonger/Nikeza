namespace Nikeza.Mobile.AppLogic

module LoginEvents =

    open System.Diagnostics
    open Nikeza.Mobile.Access.Events
    open Nikeza.Mobile.UILogic.Login

    let addNavigation sideEffects =
 
        let handle = function
            | LoggedIn    provider    -> Debug.WriteLine(sprintf "\nRequest: Navigate to Portal\n %A" provider)
            | LoginFailed credentials -> Debug.WriteLine(sprintf "\nLogin failed\n %A" credentials.Email)

        let handlers = handle::sideEffects.ForLoginAttempt

        { sideEffects with SideEffects.ForLoginAttempt= handlers }

module Login =

    open Nikeza.Mobile.UILogic.Login
    open LoginEvents

    let dependencies =

        let sideEffects = addNavigation { ForLoginAttempt= [] }
    
        { SideEffects= sideEffects }