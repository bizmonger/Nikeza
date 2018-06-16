module Nikeza.Mobile.Profile.In

open Nikeza.Common
open Nikeza.Mobile.Profile.Events
open Attempt

module Editor =

    module Validate =
        open Commands.ProfileEditor.Validate
        open Commands.ProfileEditor
        
        type private ValidateWorkflow = ValidateCommand -> ProfileValidateEvent nonempty

        let workflow : ValidateWorkflow = function
            ValidateCommand.Execute form -> 
                                    form |> Editor.validate 
                                         |> ResultOf.Editor.Validate
                                         |> Are.Editor.Validate.events

    //module Save =
    //    open Commands.ProfileEditor.Save2
    //    open Commands.ProfileEditor
    //    open Nikeza.DataTransfer

    //    type private SaveWorkflow = Result<Profile, ValidatedProfile error> -> SaveProfileEvent nonempty

    //    let events : SaveWorkflow = 
    //        fun saveFn -> function
    //        SaveCommand.Save profile -> 
    //                         profile |> saveFn
    //                                 |> ResultOf.Editor.Save
    //                                 |> Are.Editor.Save.events

module DataSources =

    module Save =
        open Commands.DataSources
        open Commands.DataSources.Save

        type private SaveWorkflow = SaveSourcesFn -> SaveCommand -> SaveDataSourcesEvent list

        let workflow : SaveWorkflow = 
            fun savefn -> function
            SaveCommand.Execute sources -> 
                                sources |> savefn
                                        |> ResultOf.Save.Execute
                                        |> Are.DataSources.Save.events    