module Nikeza.Mobile.Access.In

open Nikeza.Access.Specification.Commands.Session
open Nikeza.Access.Specification.Workflows
open Nikeza.Access.Specification.Commands
open Logic

module ValidateRegistration =
    open Registration.Validate
    
    let workflow : ValidateWorkflow = function
        Registration.Validate
                    .Execute form -> 
                             form |> Registration.validate
                                  |> ResultOf.Validate.Executed
                                  |> Are.Registration.Validation.events