module Workflows

open Commands
open Events

type Workflow = Command -> PortfolioEvent list

let handle : Workflow = fun command ->  command |> function
    | FeatureLink   response -> response |> function
                                            | Ok    linkId ->     [LinkFeatured         linkId]
                                            | Error linkId ->     [LinkFeaturedFailed   linkId]
                                                                                        
    | UnfeatureLink response -> response |> function                                    
                                            | Ok    linkId ->     [LinkUnfeatured       linkId]
                                            | Error linkId ->     [LinkUnfeaturedFailed linkId]
                                                                  
    | FeatureTopics response -> response |> function              
                                            | Ok    topicIds ->   [TopicsFeatured       topicIds]
                                            | Error topicIds ->   [TopicsFeaturedFailed topicIds]
    
    | View          response -> response |> function
                                            | Ok    provider   -> [PortfolioReturned    provider]
                                            | Error providerId -> [PortfolioFailed      providerId]