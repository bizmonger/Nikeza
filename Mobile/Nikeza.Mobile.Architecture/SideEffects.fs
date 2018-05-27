namespace Nikeza.Access.Specification

module Registration =

    open Events
    open Nikeza.Access.Specification.Attempt

    type Implementation = {
        Submit : Submit
    }

    type SideEffects = {
        ForRegistrationSubmission : (RegistrationSubmissionEvent -> unit) list
    }

    type Dependencies = {
        Implementation : Implementation
        SideEffects    : SideEffects
    }


module Login =

    open Nikeza.Common
    open Attempt
    open Events

    type Implementation =  { 
        Login : Login
    }

    type SideEffects =  { 
        ForLoginAttempt : (LoginEvent -> unit) nonempty 
    }

    type Dependencies = { 
        Implementation : Implementation
        SideEffects    : SideEffects
    }