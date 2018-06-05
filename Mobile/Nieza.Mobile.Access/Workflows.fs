module Nikeza.Mobile.Access.In

open Nikeza.Access.Specification.Commands.Session
open Nikeza.Access.Specification.Workflows
open Nikeza.Access.Specification.Commands
open Logic

module ValidateRegistration =
    open Registration.Validate
    open Nikeza.Access.Specification.Commands.Registration
    
    let workflow : ValidateWorkflow = function
        Validate form -> 
                 form |> Registration.validate
                      |> ResultOf.Validate.Executed
                      |> Are.Validation.events