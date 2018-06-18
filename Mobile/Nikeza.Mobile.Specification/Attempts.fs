namespace Nikeza.Access.Specification

open Nikeza
open DataTransfer

module Attempts =

    type SubmitAttempt = ValidatedForm -> Result<DataTransfer.Profile,  ValidatedForm>
    type LoginAttempt =  Credentials   -> Result<Provider option, Credentials>
    type LogoutAttempt = Provider      -> Result<Provider, Provider>