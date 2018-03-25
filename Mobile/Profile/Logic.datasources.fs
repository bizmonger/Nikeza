module Are.DataSources

module Save =
    open Nikeza.Mobile.Profile.Commands.DataSources.Save
    open Nikeza.DataTransfer

    type private SaveDataSources = ResultOf.Save -> Result<Profile, DataSourceSubmit list> list

    let events : SaveDataSources = function
        ResultOf.Save.Execute result -> [result]