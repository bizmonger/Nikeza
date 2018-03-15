namespace Nikeza.Mobile.AppLogic

module LoginEvents =

    open Nikeza.Mobile.Profile.Events
    open System.Diagnostics

    //module Login =

    //    open Nikeza.Mobile.UILogic.Registration

    //    let addResponders sideEffects =
 
    //        let handle = function
    //            | LoggedIn    provider    -> Debug.WriteLine(sprintf "Request: Navigate to Portal\n %A" provider)
    //            | LoginFailed credentials -> Debug.WriteLine(sprintf "Login failed\n %A" credentials.Email)

    //        let handlers = handle::sideEffects.ForRegistrationSubmission

    //        { sideEffects with SideEffects.ForRegistrationSubmission= handlers }