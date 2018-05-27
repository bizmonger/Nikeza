namespace Nikeza.Access.Specification

open Nikeza
open DataTransfer
open Common
open Events

module Try =

    type Login =  Credentials -> Result<Provider option, Credentials>
    type Logout = Provider    -> Result<Provider, Provider>

    type Submit = ValidatedForm -> Result<DataTransfer.Profile,  ValidatedForm>


module Workflows =

    open Nikeza.Access.Specification.Commands

    type SubmitWorkflow =   Try.Submit -> Registration.Command -> RegistrationSubmissionEvent nonempty

    type SessionWorkflow =  Try.Login  -> LoginCommand -> LoginEvent nonempty

    type ValidateWorkflow = Registration.Validate -> RegistrationValidationEvent nonempty

    type LogoutWorkflow =   Try.Logout -> LogoutCommand -> LogoutEvent nonempty


module Submission =
    open Commands.Registration.Submit

    type RegistrationSubmission = ResultOf.Submit -> RegistrationSubmissionEvent nonempty


module Validation =
    open Commands.Registration.Validate

    type RegistrationValidation = ResultOf.Validate -> RegistrationValidationEvent nonempty