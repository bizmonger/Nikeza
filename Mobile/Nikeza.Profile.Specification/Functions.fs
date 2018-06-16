namespace Nikeza.Portal.Specification

open Nikeza.DataTransfer
open Nikeza.Common
open Events

module ProfileEditor =
    
    module ResultOf =

        type AddTopic =    Executed of Result<unit,Topic>
        type RemoveTopic = Executed of Result<unit,Topic>
        type Save =        Executed of Result<ValidatedProfile, EditedProfile>

    module Submission =

        type SubmitTopicAddition = ResultOf.AddTopic    -> AddTopicEvent    nonempty
        type SubmitTopicRemoval =  ResultOf.RemoveTopic -> RemoveTopicEvent nonempty
        type SubmitProfile =       ResultOf.Save        -> SaveProfileEvent nonempty