module Commands

open Nikeza.DataTransfer
open Nikeza.Common

type Command =
    | FeatureLink    of Result<LinkId, LinkId>
    | UnfeatureLink  of Result<LinkId, LinkId>
    | FeatureTopics  of Result<TopicId list, TopicId list>
    | View           of Result<Provider, ProviderId>