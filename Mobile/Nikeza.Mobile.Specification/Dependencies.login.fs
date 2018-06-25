namespace Nikeza.Access.Specification

open Attempt
open Events
open Nikeza.Common

module Login =

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