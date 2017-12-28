module Commands

open Nikeza.DataTransfer
open Registration

type RegistrationCommand =
    | Validate of Registration.UnvalidatedForm
    | Submit   of Registration.ValidatedForm

type SessionCommand =                          
    | Login  of Credentials
    | Logout 

type EditCommand =                      
    | Validate of EditedProfile
    | Save     of ValidatedProfile
                       
    module ResultOf =
        type Editor =
            | Validate of Result<ValidatedProfile, EditedProfile>
            | Save     of Result<Profile, ValidatedProfile>

        type Session =
            | Login  of Result<Provider, Credentials>
            | Logout of Result<unit, unit>

        type Registration =            
            | Submit   of Result<Nikeza.DataTransfer.Profile, Registration.ValidatedForm>
            | Validate of Result<ValidatedForm, UnvalidatedForm>
