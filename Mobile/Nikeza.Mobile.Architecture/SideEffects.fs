namespace Nikeza.Access.Specification

module Login =

    open Nikeza.Common
    open Try
    open Events

    type SideEffects =  { 
        ForLoginAttempt : (LoginEvent -> unit) nonempty 
    }

    type Implementation =  { 
        Login : LoginFn
    }

    type Dependencies = { 
        Implementation : Implementation
        SideEffects    : SideEffects
    }

module Registration =
    open Events
    open Nikeza.Access.Specification.Try

    type Implementation = {
        Submit : SubmitFn
    }

    type SideEffects = {
        ForRegistrationSubmission : (RegistrationSubmissionEvent -> unit) list
    }

    type Dependencies = {
        Implementation : Implementation
        SideEffects    : SideEffects
    }