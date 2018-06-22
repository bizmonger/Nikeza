namespace Nikeza.Access.Specification

open Nikeza
open Common
open Events
open DataTransfer
    

module Workflows =

    type ValidateWorkflow = UnvalidatedForm -> RegistrationValidationEvent nonempty

module Session =

    type HandleLogin =  Result<Provider option, Credentials> -> LoginEvent  nonempty
    type HandleLogout = Result<Provider, Provider>           -> LogoutEvent nonempty


module Submission =

    type RegistrationSubmission = Result<Profile, ValidatedForm> -> RegistrationSubmissionEvent nonempty


module Validation =
    
    type ValidateForm = UnvalidatedForm -> Result<ValidatedForm, UnvalidatedForm>

    type RegistrationValidation = Result<ValidatedForm, UnvalidatedForm> -> RegistrationValidationEvent nonempty