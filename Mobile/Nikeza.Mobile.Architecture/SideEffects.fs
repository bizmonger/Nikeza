namespace Nikeza.Access.Specification

module Registration =

    open Events
    open Nikeza.Access.Specification.Attempt

    type Attempt = {
        Submit : Submit
    }

    type SideEffects = {
        ForRegistrationSubmission : (RegistrationSubmissionEvent -> unit) list
    }

    type Dependencies = {
        Attempt : Attempt
        SideEffects    : SideEffects
    }


module Login =

    open Nikeza.Common
    open Attempt
    open Events

    type Attempt =  { 
        Login : Login
    }

    type SideEffects =  { 
        ForLoginAttempt : (LoginEvent -> unit) nonempty 
    }

    type Dependencies = { 
        Attempt     : Attempt
        SideEffects : SideEffects
    }