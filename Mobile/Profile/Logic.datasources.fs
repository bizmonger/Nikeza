namespace Nikeza.Mobile.Profile.DataSources

module Save =

    open Nikeza.Mobile.Profile.Events
    open Nikeza.DataTransfer
    open Nikeza.Common

    type private SaveDataSources = Result<Nikeza.DataTransfer.Profile, (DataSourceSubmit list) error> -> SaveDataSourcesEvent list

    let toEvents : SaveDataSources = function
        result -> []