module Nikeza.Mobile.Profile.Query

open Nikeza.Mobile.Profile.Events
open Nikeza.Common

type TopicsFn = unit-> GetTopicsEvent

let topics : TopicsFn =
    fun _ -> GetTopicsFailed "Not Implemented..."