namespace Nikeza.Mobile.Access.In

open Nikeza.Access.Specification.Commands
open Nikeza.Access.Specification.Workflows
open Nikeza.Access.Specification.Commands.Registration
open Logic

module ValidateRegistration =

    open Registration.Validate
    
    let workflow : ValidateWorkflow = function
        Validate form -> 
                 form |> Registration.validate
                      |> ResultOf.Validate.Executed
                      |> Are.Validation.events