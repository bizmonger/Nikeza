namespace Nikeza.Access.Specification.Commands

open Nikeza
open DataTransfer
open Nikeza.Access.Specification

type LoginCommand =  Login  of Credentials
type LogoutCommand = Logout of Provider


module Registration =

    type Validate = Execute of UnvalidatedForm
    type Attach =   Attach  of ValidatedForm

    module Validate =
        module ResultOf = type Validate = Executed of Result<ValidatedForm, UnvalidatedForm>

    module Submit =
        module ResultOf = type Submit =   Executed of Result<Profile, ValidatedForm>


module Session =

    module ResultOf =

        type Login =  Login  of Result<Provider option, Credentials>
        type Logout = Logout of Result<Provider, Provider>