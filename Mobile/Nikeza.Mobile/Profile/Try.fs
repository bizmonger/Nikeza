module Try

open Nikeza.DataTransfer

type private ValidatedForm =   Nikeza.Mobile.Profile.Registration.ValidatedForm

type SubmitFn = ValidatedForm    -> Result<Profile, ValidatedForm>
type LoginFn =  Credentials      -> Result<Provider, Credentials>
type LogoutFn = unit             -> Result<unit, unit>
type SaveFn =   ValidatedProfile -> Result<Profile, ValidatedProfile>

let submit : SubmitFn = 
    fun registration -> Error registration

let internal logout : LogoutFn = 
    fun () -> Error ()

let internal login : LoginFn = 
    fun credentials -> Error credentials

let internal save : SaveFn = 
    fun profile -> Error profile