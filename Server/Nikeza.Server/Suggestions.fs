namespace Nikeza.Server

module Suggestions =

    open Nikeza.Server.Topics.Marketing
    open Topics.Other
    open SuggestionFinder
    let getSuggestions (text:string) =

        getSuggestions text <| StackOverflow.CachedTags.Instance()
         |> List.append <| getSuggestions text marketingTopics
         |> List.append <| getSuggestions text romanceTopics
         |> List.append <| getSuggestions text beautyTopics
         |> List.append <| getSuggestions text fitnessTopics
         |> List.append <| getSuggestions text selfImprovmentTopics