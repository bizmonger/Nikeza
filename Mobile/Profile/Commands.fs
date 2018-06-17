module Nikeza.Mobile.Profile.Commands

open Nikeza.DataTransfer

module ProfileEditor =

    type ValidateCommand = Execute of EditedProfile
    type SaveCommand =     Save    of ValidatedProfile

    //module Validate =
    //    module ResultOf =
    //        type Editor =  Validate of Result<ValidatedProfile, EditedProfile>


module DataSources =

    type SaveCommand = Execute of DataSourceSubmit list