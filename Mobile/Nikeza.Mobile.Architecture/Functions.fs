namespace Nikeza.Access.Specification

open Nikeza
open DataTransfer
open Common
open Events

module Try =

    type LoginFn =  Credentials -> Result<Provider option, Credentials>
    type LogoutFn = Provider    -> Result<Provider, Provider>

    type SubmitFn = ValidatedForm -> Result<DataTransfer.Profile,  ValidatedForm>

module Workflows =

    open Try
    open Nikeza.Access.Specification.Commands

    type SubmitWorkflow =   SubmitFn -> Registration.Command -> RegistrationSubmissionEvent nonempty

    type SessionWorkflow =  LoginFn  -> LoginCommand -> LoginEvent nonempty

    type ValidateWorkflow = Registration.Validate -> RegistrationValidationEvent nonempty

    type LogoutWorkflow =   LogoutCommand -> LogoutEvent nonempty