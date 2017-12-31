module Nikeza.Mobile.Profile.Commands

open Nikeza.DataTransfer

type UnvalidatedForm = Nikeza.Mobile.Profile.Registration.UnvalidatedForm
type ValidatedForm =   Nikeza.Mobile.Profile.Registration.ValidatedForm

type RegistrationCommand =
    | Validate of UnvalidatedForm
    | Submit   of ValidatedForm

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
            | Submit   of Result<Nikeza.DataTransfer.Profile, ValidatedForm>
            | Validate of Result<ValidatedForm, UnvalidatedForm>