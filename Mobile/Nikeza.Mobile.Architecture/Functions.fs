namespace Nikeza.Access.Specification

open Nikeza
open DataTransfer
open Common
open Commands.Registration
open Events

module Functions =

    type LoginFn =  Credentials -> Result<Provider option, Credentials>
    type LogoutFn = Provider    -> Result<Provider, Provider>

    type SubmitFn = ValidatedForm -> Result<DataTransfer.Profile,  ValidatedForm>

    type SubmitWorkflow = SubmitFn -> Command -> RegistrationSubmissionEvent nonempty