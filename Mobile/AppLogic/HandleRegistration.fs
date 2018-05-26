namespace Nikeza.Mobile.AppLogic

module RegistrationEvents =

    open System.Diagnostics
    open Nikeza.Mobile.AppLogic.Specification.Access
    open Nikeza.Access.Specification.Events
    open Nikeza.Mobile.UILogic.Registration

    module Register =
    
        let appendNavigation : Registration.AddSideEffectsFn =

            fun _ sideEffects ->
 
                let handle = function
                    | RegistrationSucceeded p -> (Debug.WriteLine(sprintf "Request: Navigate to Portal\n %A" p))
                    | RegistrationFailed    _ -> ()

                let handlers = handle::sideEffects.ForRegistrationSubmission

                { sideEffects with SideEffects.ForRegistrationSubmission= handlers }