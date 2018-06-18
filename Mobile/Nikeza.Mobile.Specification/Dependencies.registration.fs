namespace Nikeza.Access.Specification

open Events
open Nikeza.Access.Specification.Attempts

module Registration =

    type Attempt = {
        Submit : SubmitAttempt
    }

    type SideEffects = {
        ForRegistrationSubmission : (RegistrationSubmissionEvent -> unit) list
    }

    type Dependencies = {
        Attempt     : Attempt
        SideEffects : SideEffects
    }