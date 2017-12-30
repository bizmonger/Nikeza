module internal Handle

open Events
open Commands
open Commands.ResultOf

module Registration =

    type private LinkResult =   ResultOf.Link   -> LinksEvent list

    let result : LinkResult =
        fun response ->
            response |> function
                        | Link.Feature   result -> 
                                         result |> function
                                                   | Ok    linkId -> [LinkFeatured       linkId]
                                                   | Error linkId -> [LinkFeaturedFailed linkId]
                        | Link.Unfeature result -> 
                                         result |> function
                                                   | Ok    linkId -> [LinkUnfeatured       linkId]
                                                   | Error linkId -> [LinkUnfeaturedFailed linkId]

module Topics =
    type private TopicsResult = ResultOf.Topics -> TopicsEvent list

    let result : TopicsResult =
        fun response ->
            response |> function
                        Topics.Feature result -> 
                                       result |> function
                                                 | Ok    topicIds -> [TopicsFeatured       topicIds]
                                                 | Error topicIds -> [TopicsFeaturedFailed topicIds]