namespace Nikeza.Portal.Specification

open Nikeza
open Nikeza.Common
open DataTransfer

module Attempt =
    

    type AddTopic =    Topic -> Result<unit,  Topic>
    type RemoveTopic = Topic -> Result<unit,  Topic>

    type SaveSources = DataSource list -> Result<unit, DataSource list>


module Attempts =

    open Nikeza.Portal.Specification.Commands

    type AddTopicAttempt =    Attempt.AddTopic    -> AddTopicCommand    -> Result<Provider option, Credentials>
    type RemoveTopicAttempt = Attempt.RemoveTopic -> RemoveTopicCommand -> Result<Provider, Provider>

    type SaveSourcesAttempt = Attempt.SaveSources -> Result<unit, DataSource list>