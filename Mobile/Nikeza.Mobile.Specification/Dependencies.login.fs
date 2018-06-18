namespace Nikeza.Access.Specification

open Attempts
open Events
open Nikeza.Common

module Login =

    type Attempt =  { 
        Login : LoginAttempt
    }

    type SideEffects =  { 
        ForLoginAttempt : (LoginEvent -> unit) nonempty 
    }

    type Dependencies = { 
        Attempt     : Attempt
        SideEffects : SideEffects
    }