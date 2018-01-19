module Nikeza.Mobile.Profile.Query

open Nikeza.Common

type PlatformsFn = unit-> Result<string list, string>
type TopicsFn =    unit-> Result<Topic list, string>
type SourcesFn =   unit-> Result<DataSourceRequest list, string>

let topics : TopicsFn =
    fun _ -> Error "Not Implemented..."
                             
let profile : SourcesFn =    
    fun _ -> Error "Not Implemented..."

let platforms : PlatformsFn =
    fun _ -> Error "Not Implemented..."