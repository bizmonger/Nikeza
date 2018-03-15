namespace Nikeza.Mobile.AppLogic

module LoginEvents =

    open System.Diagnostics
    open Nikeza.Mobile.Profile.Events
    open Nikeza.Mobile.UILogic.Login

    let addTo sideEffects =
 
        let handle = function
            | LoggedIn    provider    -> Debug.WriteLine(sprintf "Request: Navigate to Portal\n %A" provider)
            | LoginFailed credentials -> Debug.WriteLine(sprintf "Login failed\n %A" credentials.Email)

        let handlers = handle::sideEffects.ForLoginAttempt

        { sideEffects with SideEffects.ForLoginAttempt= handlers }