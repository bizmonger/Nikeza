module Nikeza.Mobile.Profile.In

open Nikeza.Common
open Nikeza.Mobile.Profile.Events
open Nikeza.Mobile.Profile
open Attempt

module Editor =

    module Validate =

        open Validate
        open Editor
        open Commands.ProfileEditor

        type private ValidateWorkflow = ValidateCommand -> ProfileValidateEvent nonempty

        let workflow : ValidateWorkflow = function
            ValidateCommand.Execute form -> 
                                    form |> validate 
                                         |> toEvents

//module DataSources =

//    module Save =

//        open Commands.DataSources
//        open Nikeza
//        open Nikeza.DataTransfer

//        type private SaveWorkflow = SaveSourcesFn -> SaveCommand -> Result<DataTransfer.Profile, ValidatedProfile error>

//        //let workflow : SaveWorkflow = 
//        //    fun savefn -> function
//        //    SaveCommand.Execute sources -> 
//        //                        sources |> savefn
//                                        //|> Nikeza.Mobile.Profile.Save.toEvents    