namespace Nikeza.Access.Specification

open Nikeza
open Common
open Events
open DataTransfer
    

module Workflows =

    type ValidateRegistration = UnvalidatedForm -> RegistrationValidationEvent nonempty

module Session =

    type HandleLoginResult =  Result<Provider option, Credentials> -> LoginEvent  nonempty
    type HandleLogoutResult = Result<Provider, Provider>           -> LogoutEvent nonempty


module Submission =

    type HandleRegistrationResult = Result<Profile, ValidatedForm> -> RegistrationSubmissionEvent nonempty


module Validation =
    
    type ValidateForm = UnvalidatedForm -> Result<ValidatedForm, UnvalidatedForm>

    type RegistrationValidation = Result<ValidatedForm, UnvalidatedForm> -> RegistrationValidationEvent nonempty