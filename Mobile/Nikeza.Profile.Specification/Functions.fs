namespace Nikeza.Portal.Specification

open Nikeza.Common
open Events

module ProfileEditor =
    
    module ResultOf =

        type AddTopic =    Executed of Result<unit,Topic>
        type RemoveTopic = Executed of Result<unit,Topic>

    module Submission =

        type AddTopicSubmission =    ResultOf.AddTopic    -> AddTopicEvent    nonempty
        type RemoveTopicSubmission = ResultOf.RemoveTopic -> RemoveTopicEvent nonempty