module Nikeza.Mobile.Profile.Query

open Nikeza.Mobile.Profile.Events

type TopicsFn = unit-> GetTopicsEvent

let topics : TopicsFn =
    fun _ -> GetTopicsFailed "Not Implemented..."