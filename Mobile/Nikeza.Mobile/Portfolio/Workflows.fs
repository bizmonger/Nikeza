module Workflows

open Commands
open Events

type private LinksWorkflow =  LinkCommand   -> LinksEvent  list
type private TopicsWorkflow = TopicsCommand -> TopicsEvent list

let handleLinks : LinksWorkflow = fun command -> command |> function
    | LinkCommand.Feature   linkId -> 
                            linkId |> Try.featureLink
                                   |> ResultOf.Link.Feature
                                   |> Are.Registration.events
                                   
    | LinkCommand.Unfeature linkId -> 
                            linkId |> Try.unfeatureLink
                                   |> ResultOf.Link.Feature
                                   |> Are.Registration.events

let handleTopics : TopicsWorkflow = fun command -> command |> function
    TopicsCommand.Feature topicIds ->
                          topicIds |> Try.featureTopics
                                   |> ResultOf.Topics.Feature
                                   |> Are.Topics.events