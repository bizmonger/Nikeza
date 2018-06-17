namespace Nikeza.Mobile.Access

open Nikeza.Access.Specification.Workflows
open Nikeza.Access.Specification.Commands.Registration
open Nikeza.Mobile.Access.Validation
open Logic.Registration

module ValidateRegistration =
    
    let workflow : ValidateWorkflow = function

        Validate form -> 
                 form |> validate
                      |> toEevents