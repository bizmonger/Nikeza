namespace Nikeza.Mobile.AppLogic

module RegistrationEvents =

    open System.Diagnostics
    open Nikeza.Mobile.Access.Events
    open Nikeza.Mobile.UILogic.Registration
    open Design.Access

    module Register =
    
        let appendNavigation : Registration.SideEffects =

            fun app sideEffects ->
 
                let handle = function
                    | RegistrationSucceeded p -> (Debug.WriteLine(sprintf "Request: Navigate to Portal\n %A" p))
                    | RegistrationFailed    _ -> ()

                let handlers = handle::sideEffects.ForRegistrationSubmission

                { sideEffects with SideEffects.ForRegistrationSubmission= handlers }