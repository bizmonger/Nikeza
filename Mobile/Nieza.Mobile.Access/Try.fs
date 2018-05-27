module Nikeza.Mobile.Access.Try

open Nikeza.Access.Specification

let submit : Try.Submit = 
    fun validatedForm -> Error validatedForm

let logout : Try.Logout = 
    fun provider -> Error provider

let login : Try.Login = 
    fun credentials -> Error credentials