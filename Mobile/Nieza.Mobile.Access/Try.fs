module Nikeza.Mobile.Access.Try

open Nikeza.Access.Specification.Try

let submit : SubmitFn = 
    fun validatedForm -> Error validatedForm

let logout : LogoutFn = 
    fun provider -> Error provider

let login : LoginFn = 
    fun credentials -> Error credentials