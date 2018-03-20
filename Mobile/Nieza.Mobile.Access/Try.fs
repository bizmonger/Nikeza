module Nikeza.Mobile.Access.Try

open Nikeza.DataTransfer
open Nikeza.Mobile.Access

type SubmitFn = ValidatedForm -> Result<Profile, ValidatedForm>
type LoginFn =  Credentials   -> Result<Provider, Credentials>
type LogoutFn = Provider      -> Result<Provider, Provider>

let submit : SubmitFn = 
    fun registration -> Error registration

let logout : LogoutFn = 
    fun p -> Error p

let login : LoginFn = 
    fun credentials -> Error credentials