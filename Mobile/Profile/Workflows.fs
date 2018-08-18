module Nikeza.Mobile.Profile.In

open Nikeza.Common
open Nikeza.Mobile.Profile.Events
open Nikeza.Mobile.Profile

module Editor =

    module Validate =

        open Validate
        open Editor
        open Nikeza.DataTransfer

        type private ValidateWorkflow = EditedProfile -> ProfileValidateEvent nonempty

        let workflow : ValidateWorkflow = function
            form -> 
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