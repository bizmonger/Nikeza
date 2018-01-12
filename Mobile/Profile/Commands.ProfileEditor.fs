module Nikeza.Mobile.Profile.Commands.ProfileEditor

//module ProfileEditor =

    type ValidateCommand = Execute of EditedProfile
    type SaveCommand =     Execute of ValidatedProfile

    module Validate =
        module ResultOf =
            type Editor =  Validate of Result<ValidatedProfile, EditedProfile>

    module Save =
        module ResultOf =
            type Editor = Save of Result<Profile, ValidatedProfile>