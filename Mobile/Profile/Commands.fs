﻿module Nikeza.Mobile.Profile.Commands

open Nikeza.DataTransfer

type UnvalidatedForm = Nikeza.Mobile.Profile.Registration.UnvalidatedForm
type ValidatedForm =   Nikeza.Mobile.Profile.Registration.ValidatedForm

type SessionCommand =                          
    | Login  of Credentials
    | Logout 

type ValidateCommand = Validate of EditedProfile
type SaveCommand = Save of ValidatedProfile

module Registration =
    type Validate =  Execute of UnvalidatedForm
    type Command =   Execute of ValidatedForm

    module Validate =
        module ResultOf = type Validate = Executed of Result<ValidatedForm, UnvalidatedForm>

    module Submit =
        module ResultOf = type Submit = Executed of Result<Nikeza.DataTransfer.Profile, ValidatedForm>

module ProfileEditor =
    module Validate =
        module ResultOf =
            type Editor =     Validate of Result<ValidatedProfile, EditedProfile>

    module Save =
        module ResultOf =
            type Editor = Save of Result<Profile, ValidatedProfile>

module ResultOf =

    type Session =
        | Login  of Result<Provider, Credentials>
        | Logout of Result<unit, unit>