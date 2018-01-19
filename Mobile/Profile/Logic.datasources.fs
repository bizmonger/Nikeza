module Are.DataSources

module Save =
    open Nikeza.Mobile.Profile.Commands.DataSources.Save
    open Nikeza.Mobile.Profile.Events

    type private SaveDataSources = ResultOf.Save -> SourcesSaveEvent list

    let events : SaveDataSources =
        fun resultOf -> 
            resultOf |> function
                        ResultOf.Save.Execute result -> 
                                              result |> function
                                                        | Ok p          -> [SourcesSaveEvent.SourcesSaved p]
                                                        | Error sources -> [SourcesSaveEvent.SourcesFailed sources]