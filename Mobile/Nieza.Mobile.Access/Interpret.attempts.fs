module Nikeza.Mobile.Access.Attempt

open Nikeza.Access.Specification.Attempts

let submit : SubmitAttempt = 
    fun validatedForm -> Error validatedForm

let logout : LogoutAttempt = 
    fun provider -> Error provider

let login : LoginAttempt = 
    fun credentials -> Error credentials