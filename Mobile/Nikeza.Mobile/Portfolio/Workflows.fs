module Workflows.Commands

open Commands
open Events

type private LinksWorkflow =  LinkCommand   -> LinksEvent  list
type private TopicsWorkflow = TopicsCommand -> TopicsEvent list

let handleLinks : LinksWorkflow = fun command -> command |> function
    | LinkCommand.Feature   linkId -> 
                            linkId |> Try.featureLink
                                   |> ResultOf.Link.Feature
                                   |> Handle.Registration.result
                                   
    | LinkCommand.Unfeature linkId -> 
                            linkId |> Try.unfeatureLink
                                   |> ResultOf.Link.Feature
                                   |> Handle.Registration.result

let handleTopics : TopicsWorkflow = fun command -> command |> function
    TopicsCommand.Feature topicIds ->
                          topicIds |> Try.featureTopics
                                   |> ResultOf.Topics.Feature
                                   |> Handle.Topics.result