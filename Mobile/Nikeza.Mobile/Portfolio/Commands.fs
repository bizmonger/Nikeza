module Nikeza.Mobile.Portfolio.Commands

open Nikeza.Common

type LinkCommand =
    | Feature   of LinkId
    | Unfeature of LinkId

type TopicsCommand =    Feature of TopicId list

module ResultOf =

    type Link =
        | Feature   of Result<LinkId, LinkId>
        | Unfeature of Result<LinkId, LinkId>

    type Topics = Feature of Result<TopicId list, TopicId list>