namespace Nikeza.Access.Specification.Commands

open Nikeza
open Nikeza.Access.Specification
open DataTransfer

type Validate = Execute of UnvalidatedForm
type Command =  Execute of ValidatedForm

type LoginCommand =  Submit of Credentials
type LogoutCommand = Logout of Provider


module Registration =

    type Validate = Execute of UnvalidatedForm
    type Command =  Execute of ValidatedForm

    module Validate =
        module ResultOf = type Validate = Executed of Result<ValidatedForm, UnvalidatedForm>

    module Submit =
        module ResultOf = type Submit = Executed of Result<Profile, ValidatedForm>


module Session =

    module ResultOf =

        type Login =  Login  of Result<Provider option, Credentials>
        type Logout = Logout of Result<Provider, Provider>