namespace Nikeza.Access.Specification

open Events
open Nikeza.Common

module Login =

    open Attempt

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