module Nikeza.Mobile.Profile.Commands

open Nikeza.DataTransfer

module ProfileEditor =

    type ValidateCommand = Execute of EditedProfile
    type SaveCommand =     Execute of ValidatedProfile

    module Validate =
        module ResultOf =
            type Editor =  Validate of Result<ValidatedProfile, EditedProfile>

    module Save =
        module ResultOf =
            type Editor = Save of Result<Profile, ValidatedProfile>


module DataSources =

    type SaveCommand = Execute of DataSourceSubmit list

    module Save =
        module ResultOf =

            type Save = Execute of Result<Nikeza.DataTransfer.Profile, DataSourceSubmit list>