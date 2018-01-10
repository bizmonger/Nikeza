module Nikeza.Mobile.Profile.Query

open Nikeza.Mobile.Profile.Events

type TopicsFn = unit-> Query

let topics : TopicsFn =
    fun _ -> TopicsFailed "Not Implemented..."