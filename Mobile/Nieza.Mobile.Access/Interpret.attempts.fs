module Nikeza.Mobile.Access.Attempt

open Nikeza.Access.Specification.Attempt

let submit : Submit = 
    fun validatedForm -> Error validatedForm

let logout : Logout = 
    fun provider -> Error provider

let login : Login = 
    fun credentials -> Error credentials