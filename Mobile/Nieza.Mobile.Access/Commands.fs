module Nikeza.Mobile.Access.Commands

open Nikeza.DataTransfer

type UnvalidatedForm = Nikeza.Mobile.Access.UnvalidatedForm
type ValidatedForm =   Nikeza.Mobile.Access.ValidatedForm

type LoginCommand =  Login  of Credentials
type LogoutCommand = Logout of Provider

module Registration =
    type Validate = Execute of UnvalidatedForm
    type Command =  Execute of ValidatedForm

    module Validate =
        module ResultOf = type Validate = Executed of Result<ValidatedForm, UnvalidatedForm>

    module Submit =
        module ResultOf = type Submit = Executed of Result<Nikeza.DataTransfer.Profile, ValidatedForm>

module Session =

    module ResultOf =

        type Login =  Login  of Result<Provider, Credentials>
        type Logout = Logout of Result<Provider, Provider>