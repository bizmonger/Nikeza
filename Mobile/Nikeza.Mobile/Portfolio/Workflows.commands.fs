module Workflows.Commands

open Commands
open Events

type private LinksWorkflow =  LinkCommand    -> LinksEvent     list
type private TopicsWorkflow = TopicsCommand  -> TopicsEvent    list

let handleLinks : LinksWorkflow = fun command -> command |> function
    | LinkCommand.Feature   linkId -> 
                            linkId |> Try.featureLink
                                   |> ResultOf.Link.Feature
                                   |> LinkResult.handle
                                   
    | LinkCommand.Unfeature linkId -> 
                            linkId |> Try.unfeatureLink
                                   |> ResultOf.Link.Feature
                                   |> LinkResult.handle

let handleTopics : TopicsWorkflow = fun command -> command |> function
    TopicsCommand.Feature topicIds ->
                          topicIds |> Try.featureTopics
                                   |> ResultOf.Topics.Feature
                                   |> TopicResult.handle