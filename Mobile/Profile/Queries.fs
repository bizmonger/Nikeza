module Nikeza.Mobile.Profile.Query

open Nikeza.Mobile.Profile.Events

type PlatformsFn = unit-> PlatformsQuery
type TopicsFn =    unit-> TopicsQuery
type SourcesFn =   unit-> SourcesQuery

let topics : TopicsFn =
    fun _ -> TopicsFailed    "Not Implemented..."
                             
let profile : SourcesFn =    
    fun _ -> SourcesFailed   "Not Implemented..."

let platforms : PlatformsFn =
    fun _ -> PlatformsFailed "Not Implemented..."