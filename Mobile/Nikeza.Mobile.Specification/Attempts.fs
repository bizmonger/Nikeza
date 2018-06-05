namespace Nikeza.Access.Specification

open Nikeza
open DataTransfer

module Attempt =

    type Submit = ValidatedForm -> Result<DataTransfer.Profile,  ValidatedForm>
    type Login =  Credentials   -> Result<Provider option, Credentials>
    type Logout = Provider      -> Result<Provider, Provider>


module Attempts =

    open Nikeza.Access.Specification.Commands

    type SubmitAttempt = Attempt.Submit -> Registration.AttachCommand -> Result<DataTransfer.Profile,  ValidatedForm>
    type LoginAttempt =  Attempt.Login  -> LoginCommand        -> Result<Provider option, Credentials>
    type LogoutAttempt = Attempt.Logout -> LogoutCommand       -> Result<Provider, Provider>