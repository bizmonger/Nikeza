module internal IO

open Registration
open Nikeza.DataTransfer

type private TrySubmit = ValidatedForm    -> Result<Profile, ValidatedForm>
type private TryLogin =  Credentials      -> Result<Provider, Credentials>
type private TryLogout = unit             -> Result<unit, unit>
type private TrySave =   ValidatedProfile -> Result<Profile, ValidatedProfile>

let trySubmit : TrySubmit = 
    fun form -> Error form

let tryLogout : TryLogout = 
    fun () -> Error ()

let tryLogin : TryLogin = 
    fun credentials -> Error credentials

let trySave : TrySave = 
    fun profile -> Error profile