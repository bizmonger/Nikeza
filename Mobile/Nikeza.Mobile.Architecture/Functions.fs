namespace Nikeza.Access.Specification

open Nikeza
open DataTransfer
open Common
open Events

module Attempt =

    type Login =  Credentials   -> Result<Provider option, Credentials>
    type Logout = Provider      -> Result<Provider, Provider>
    type Submit = ValidatedForm -> Result<DataTransfer.Profile,  ValidatedForm>


module Attempts =

    open Nikeza.Access.Specification.Commands

    type SubmitAttempt = Attempt.Submit -> Registration.Command -> Result<DataTransfer.Profile,  ValidatedForm>
    type LoginAttempt =  Attempt.Login  -> LoginCommand         -> Result<Provider option, Credentials>
    type LogoutAttempt = Attempt.Logout -> LogoutCommand        -> Result<Provider, Provider>
    

module Workflows =
    open Nikeza.Access.Specification.Commands
    type ValidateWorkflow = Registration.Validate -> RegistrationValidationEvent nonempty


module Session =
    open Nikeza.Access.Specification.Commands.Session

    type HandleLogin =  ResultOf.Login  -> LoginEvent  nonempty
    type HandleLogout = ResultOf.Logout -> LogoutEvent nonempty


module Submission =
    open Commands.Registration.Submit

    type RegistrationSubmission = ResultOf.Submit -> RegistrationSubmissionEvent nonempty


module Validation =
    open Commands.Registration.Validate

    type RegistrationValidation = ResultOf.Validate -> RegistrationValidationEvent nonempty