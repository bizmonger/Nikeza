module Nikeza.Mobile.Profile.In

open Nikeza.Mobile.Profile.Events
open Try

module Editor =

    module Validate =
        open Commands.ProfileEditor.Validate
        open Commands.ProfileEditor
        
        type private ValidateWorkflow = ValidateCommand -> ProfileValidateEvent list

        let workflow : ValidateWorkflow = function
            ValidateCommand.Execute form -> 
                                    form |> Editor.validate 
                                            |> ResultOf.Editor.Validate
                                            |> Are.Editor.Validate.events

    module Save =
        open Commands.ProfileEditor.Save
        open Commands.ProfileEditor

        type private SaveWorkflow = SaveProfileFn -> SaveCommand -> SaveProfileEvent list

        let workflow : SaveWorkflow = 
            fun saveFn -> function
            SaveCommand.Execute profile -> 
                                profile |> saveFn
                                        |> ResultOf.Editor.Save
                                        |> Are.Editor.Save.events

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