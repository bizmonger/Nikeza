namespace Nikeza.Access.Specification

open Events
open Nikeza.Access.Specification.Attempt

module Registration =

    type Attempt = {
        Submit : Submit
    }

    type SideEffects = {
        ForRegistrationSubmission : (RegistrationSubmissionEvent -> unit) list
    }

    type Dependencies = {
        Attempt     : Attempt
        SideEffects : SideEffects
    }