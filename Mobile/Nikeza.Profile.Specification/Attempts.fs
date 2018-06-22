namespace Nikeza.Portal.Specification

open Nikeza.Common

module Attempt =
    

    type AddTopic =    Topic -> Result<unit,  Topic>
    type RemoveTopic = Topic -> Result<unit,  Topic>

    type SaveSources = DataSource list -> Result<unit, DataSource list>