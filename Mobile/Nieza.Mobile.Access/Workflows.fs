namespace Nikeza.Mobile.Access

open Nikeza.Access.Specification.Workflows
open Nikeza.Mobile.Access.Validation
open Logic.Registration

module ValidateRegistration =
    
    let workflow : ValidateRegistration = function

        form -> 
        form |> validate
             |> toEevents