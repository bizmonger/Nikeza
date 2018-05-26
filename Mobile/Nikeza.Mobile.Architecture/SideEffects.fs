namespace Nikeza.Access.Specification

module Login =

    open Nikeza.Common
    open Functions
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