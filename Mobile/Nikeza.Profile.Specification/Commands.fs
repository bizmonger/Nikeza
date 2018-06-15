module Nikeza.Portal.Specification.Commands

open Nikeza.Common

type AddTopicCommand =    AddTopic    of Topic
type RemoveTopicCommand = RemoveTopic of Topic