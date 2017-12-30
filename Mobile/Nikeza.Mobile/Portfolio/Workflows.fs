module Execute

open Commands
open Events

module Links =
    type private LinksWorkflow = LinkCommand -> LinksEvent  list

    let workflow : LinksWorkflow = fun command -> command |> function
        | LinkCommand.Feature   linkId -> 
                                linkId |> Try.featureLink
                                       |> ResultOf.Link.Feature
                                       |> Are.Registration.events
                                   
        | LinkCommand.Unfeature linkId -> 
                                linkId |> Try.unfeatureLink
                                       |> ResultOf.Link.Feature
                                       |> Are.Registration.events

module Topics =
    type private TopicsWorkflow = TopicsCommand -> TopicsEvent list

    let workflow : TopicsWorkflow = fun command -> command |> function
        TopicsCommand.Feature topicIds ->
                              topicIds |> Try.featureTopics
                                       |> ResultOf.Topics.Feature
                                       |> Are.Topics.events