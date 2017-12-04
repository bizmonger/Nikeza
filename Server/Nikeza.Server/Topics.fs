module Nikeza.Server.Topics

open SuggestionFinder

let marketingTopics = [ "advertising"
                        "branding"
                        "competition"
                        "consumer-behavior"
                        "customer-relationships"
                        "distribution-channels"
                        "global-marketing"
                        "innovation"
                        "legal"
                        "marketing-communications"
                        "marketing-strategy"
                        "metrics"
                        "product-development"
                        "organizational-buying-behavior"
                        "organizational-processes"
                        "pricing"
                        "promotions"
                        "product-management"
                        "service-management"
                        "research-tools"
                        "research-methods"
                        "sales"
                        "social-networks"
                        "supply-chain"
                      ]

let getSuggestions (text:string) =

    let stackoverflowSuggestions =  getSuggestions text <| StackOverflow.CachedTags.Instance()
    let marketingSuggestions =      getSuggestions text marketingTopics
    stackoverflowSuggestions @ marketingSuggestions