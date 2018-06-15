namespace Nikeza.Portal.Specification

open Nikeza.Common

module Events =

    type AddTopicEvent =
        | TopicAdded       of Topic
        | FailedToAddTopic of Topic

    type RemoveTopicEvent =
        | TopicRemoved        of Topic
        | FailedToRemoveTopic of Topic