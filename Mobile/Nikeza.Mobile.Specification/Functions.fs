namespace Nikeza.Access.Specification

open Nikeza
open Common
open Events
    

module Workflows =
    open Nikeza.Access.Specification.Commands
    type ValidateWorkflow = Registration.ValidateCommand -> RegistrationValidationEvent nonempty

module Session =
    open Nikeza.Access.Specification.Commands.Session

    type HandleLogin =  ResultOf.Login  -> LoginEvent  nonempty
    type HandleLogout = ResultOf.Logout -> LogoutEvent nonempty


module Submission =
    open Commands.Registration.Submit

    type RegistrationSubmission = ResultOf.Submit -> RegistrationSubmissionEvent nonempty


module Validation =
    open Commands.Registration.Validate
    
    type ValidateForm = UnvalidatedForm -> Result<ValidatedForm, UnvalidatedForm>

    type RegistrationValidation = ResultOf.Validate -> RegistrationValidationEvent nonempty