namespace Nikeza.Portal.Specification

open Nikeza
open Nikeza.Common
open DataTransfer

module Attempt =
    

    type AddTopic =    Topic -> Result<unit,  Topic>
    type RemoveTopic = Topic -> Result<unit,  Topic>


module Attempts =

    open Nikeza.Portal.Specification.Commands

    type AddTopicttempt =    Attempt.AddTopic    -> AddTopicCommand    -> Result<Provider option, Credentials>
    type RemoveTopicttempt = Attempt.RemoveTopic -> RemoveTopicCommand -> Result<Provider, Provider>