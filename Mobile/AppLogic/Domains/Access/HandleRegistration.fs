namespace Nikeza.Mobile.AppLogic

module RegistrationEvents =

    open System.Diagnostics
    open Nikeza.Mobile.AppLogic.Specification.Access
    open Nikeza.Access.Specification.Events
    open Nikeza.Access.Specification.Registration

    module Register =
    
        let appendNavigation : Registration.AddSideEffects =

            fun _ sideEffects ->
 
                let handle = function
                    | RegistrationSucceeded p -> Debug.WriteLine(sprintf "Request: Navigate to Portal\n %A" p)
                    | RegistrationFailed    _ -> Debug.WriteLine("Registration failed")

                let handlers = handle::sideEffects.ForRegistrationSubmission

                { sideEffects with SideEffects.ForRegistrationSubmission= handlers }