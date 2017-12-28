module internal IO

open Registration
open Nikeza.DataTransfer

type TrySubmit = ValidatedForm -> Result<Profile, ValidatedForm>
type TryLogin =  Credentials   -> Result<Provider, Credentials>
type TryLogout = unit          -> Result<unit, unit>

let trySubmit : TrySubmit = 
    fun form -> Error form

let tryLogout : TryLogout = 
    fun () -> Error ()

let tryLogin : TryLogin = 
    fun credentials -> Error credentials