module Nikeza.Mobile.Access.Try

open Nikeza.DataTransfer
open Nikeza.Mobile.Access

type SubmitFn = ValidatedForm -> Result<Profile,  ValidatedForm>
type LoginFn =  Credentials   -> Result<Provider, Credentials>
type LogoutFn = Provider      -> Result<Provider, Provider>

let submit : SubmitFn = 
    fun validatedForm -> Error validatedForm

let logout : LogoutFn = 
    fun provider -> Error provider

let login : LoginFn = 
    fun credentials -> Error credentials