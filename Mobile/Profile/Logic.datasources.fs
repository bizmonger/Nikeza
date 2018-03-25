module Are.DataSources

module Save =
    open Nikeza.Mobile.Profile.Commands.DataSources.Save
    open Nikeza.Mobile.Profile.Events

    type private SaveDataSources = ResultOf.Save -> SaveDataSourcesEvent list

    let events : SaveDataSources = function
        ResultOf.Save.Execute result -> [result]