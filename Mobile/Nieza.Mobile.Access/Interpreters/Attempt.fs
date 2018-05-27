module Nikeza.Mobile.Access.Attempt

open Nikeza.Access.Specification

let submit : Attempt.Submit = 
    fun validatedForm -> Error validatedForm

let logout : Attempt.Logout = 
    fun provider -> Error provider

let login : Attempt.Login = 
    fun credentials -> Error credentials