namespace Nikeza.Mobile.Profile.DataSources

module Save =

    open Nikeza.Mobile.Profile.Events
    open Nikeza.DataTransfer

    type private SaveDataSources = Result<Nikeza.DataTransfer.Profile, DataSourceSubmit list> -> SaveDataSourcesEvent list

    let toEvents : SaveDataSources = function
        result -> [result]