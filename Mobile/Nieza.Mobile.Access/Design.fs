namespace Nikeza.Mobile.Access

open Nikeza.DataTransfer

(* LANGUAGE *)
type Email =      Email      of string
type Password =   Password   of string
type EntityName = EntityName of string
type FirstName =  FirstName  of string
type LastName =   LastName   of string

type Form = {
    Email:    Email
    Password: Password
    Confirm:  Password
}

type UnvalidatedForm = { Form : Form }
type ValidatedForm =   { Form : Form }

(* FUNCTIONS *)
type SubmitFn = ValidatedForm -> Result<Profile,  ValidatedForm>
type LoginFn =  Credentials   -> Result<Provider option, Credentials>
type LogoutFn = Provider      -> Result<Provider, Provider>