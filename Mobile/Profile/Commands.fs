module Nikeza.Mobile.Profile.Commands

open Nikeza.DataTransfer

type UnvalidatedForm = Nikeza.Mobile.Profile.Registration.UnvalidatedForm
type ValidatedForm =   Nikeza.Mobile.Profile.Registration.ValidatedForm

type LoginCommand =  Login  of Credentials
type LogoutCommand = Logout of Provider

module Registration =
    type Validate =  Execute of UnvalidatedForm
    type Command =   Execute of ValidatedForm

    module Validate =
        module ResultOf = type Validate = Executed of Result<ValidatedForm, UnvalidatedForm>

    module Submit =
        module ResultOf = type Submit = Executed of Result<Nikeza.DataTransfer.Profile, ValidatedForm>

module ProfileEditor =

    type ValidateCommand = Execute of EditedProfile
    type SaveCommand =     Execute of ValidatedProfile

    module Validate =
        module ResultOf =
            type Editor =  Validate of Result<ValidatedProfile, EditedProfile>

    module Save =
        module ResultOf =
            type Editor = Save of Result<Profile, ValidatedProfile>

module ResultOf =

    type Login =  Login  of Result<Provider, Credentials>
    type Logout = Logout of Result<Provider, Provider>


module DataSources =

    type SaveCommand = Execute of DataSourceSubmit list

    module Save =
        module ResultOf =

            type Save = Execute of Result<Nikeza.DataTransfer.Profile, DataSourceSubmit list>