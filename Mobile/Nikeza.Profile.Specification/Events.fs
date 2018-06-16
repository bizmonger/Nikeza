namespace Nikeza.Portal.Specification

open Nikeza.Common

module Events =
    open Nikeza

    type AddTopicEvent =
        | TopicAdded       of Topic
        | FailedToAddTopic of Topic

    type RemoveTopicEvent =
        | TopicRemoved        of Topic
        | FailedToRemoveTopic of Topic

    type SaveProfileEvent =
        | SaveProfileSucceeded of DataTransfer.Profile
        | SaveProfileFailed    of DataTransfer.Profile error