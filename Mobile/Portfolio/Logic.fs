module internal Are

open Nikeza.Mobile.Portfolio.Events
open Nikeza.Mobile.Portfolio.Commands
open Nikeza.Mobile.Portfolio.Commands.ResultOf

module Registration =

    type private LinkResult = ResultOf.Link -> LinksEvent list

    let events : LinkResult = function
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

    let events : TopicsResult = function
        Topics.Feature result -> 
                       result |> function
                                 | Ok    topicIds -> [TopicsFeatured       topicIds]
                                 | Error topicIds -> [TopicsFeaturedFailed topicIds]