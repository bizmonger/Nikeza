module internal TopicResult

open Events
open Commands
open Commands.ResultOf

type private Handle = ResultOf.Topics -> TopicsEvent list

let handle : Handle =
    fun response ->
        response |> function
                    | Topics.Feature result -> 
                                     result |> function
                                               | Ok    topicIds -> [TopicsFeatured       topicIds]
                                               | Error topicIds -> [TopicsFeaturedFailed topicIds]