module Nikeza.Mobile.Profile.Commands

open Nikeza.DataTransfer

type UnvalidatedForm = Nikeza.Mobile.Profile.Registration.UnvalidatedForm
type ValidatedForm =   Nikeza.Mobile.Profile.Registration.ValidatedForm

module Registration =
    type Validate = Execute of UnvalidatedForm
    type Submit =   Execute   of ValidatedForm

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

        type Submit =            
            Executed of Result<Nikeza.DataTransfer.Profile, ValidatedForm>

        type Validation =
            Executed of Result<ValidatedForm, UnvalidatedForm>