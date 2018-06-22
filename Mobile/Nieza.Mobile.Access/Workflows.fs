namespace Nikeza.Mobile.Access

open Nikeza.Access.Specification.Workflows
open Nikeza.Mobile.Access.Validation
open Logic.Registration

module Registration =
    
    let validate : ValidateRegistration = function

        form -> 
        form |> validate
             |> toEevents