module internal Try

open Nikeza.DataTransfer

type ValidatedForm =   Nikeza.Mobile.Profile.Registration.ValidatedForm
type UnvalidatedForm = Nikeza.Mobile.Profile.Registration.UnvalidatedForm

type private TrySubmit = ValidatedForm    -> Result<Profile, ValidatedForm>
type private TryLogin =  Credentials      -> Result<Provider, Credentials>
type private TryLogout = unit             -> Result<unit, unit>
type private TrySave =   ValidatedProfile -> Result<Profile, ValidatedProfile>

let submit : TrySubmit = 
    fun form -> Error form

let logout : TryLogout = 
    fun () -> Error ()

let login : TryLogin = 
    fun credentials -> Error credentials

let save : TrySave = 
    fun profile -> Error profile