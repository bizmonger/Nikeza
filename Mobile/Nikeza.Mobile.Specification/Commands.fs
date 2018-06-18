namespace Nikeza.Access.Specification.Commands

open Nikeza
open Nikeza.Access.Specification


module Registration =

    type ValidateCommand = Validate of UnvalidatedForm
    type AttachCommand =   Attach   of ValidatedForm