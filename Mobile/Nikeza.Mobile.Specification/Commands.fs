namespace Nikeza.Access.Specification.Commands

open Nikeza
open DataTransfer
open Nikeza.Access.Specification

type LoginCommand =  Login  of Credentials
type LogoutCommand = Logout of Provider


module Registration =

    type ValidateCommand = Validate of UnvalidatedForm
    type AttachCommand =   Attach   of ValidatedForm