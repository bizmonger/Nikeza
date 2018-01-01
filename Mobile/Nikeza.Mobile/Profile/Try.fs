module Try

open Nikeza.DataTransfer

type private ValidatedForm =   Nikeza.Mobile.Profile.Registration.ValidatedForm

type SubmitRegistration = ValidatedForm    -> Result<Profile, ValidatedForm>
type Login =              Credentials      -> Result<Provider, Credentials>
type Logout =             unit             -> Result<unit, unit>
type Save =               ValidatedProfile -> Result<Profile, ValidatedProfile>

let submit : SubmitRegistration = 
    fun form -> Error form

let internal logout : Logout = 
    fun () -> Error ()

let internal login : Login = 
    fun credentials -> Error credentials

let internal save : Save = 
    fun profile -> Error profile