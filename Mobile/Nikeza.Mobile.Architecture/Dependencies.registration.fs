namespace Nikeza.Access.Specification

open Events
open Nikeza.Common

module Registration =

    open Nikeza.Access.Specification.Attempt

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