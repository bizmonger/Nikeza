namespace Nikeza.Access.Specification

open Nikeza
open DataTransfer

module Attempt =

    type Submit = ValidatedForm -> Result<DataTransfer.Profile,  ValidatedForm>
    type Login =  Credentials   -> Result<Provider option, Credentials>
    type Logout = Provider      -> Result<Provider, Provider>