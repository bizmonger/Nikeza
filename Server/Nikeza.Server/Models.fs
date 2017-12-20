module Nikeza.Server.Model

open System
open Literals
open Nikeza.Common

type Platform =
    | YouTube
    | WordPress
    | StackOverflow
    | Medium
    | RSSFeed
    | Other

type ContentType = 
    | Article
    | Video
    | Answer
    | Podcast
    | Unknown

let contentTypeFromString = function
    | ArticleText -> Article
    | VideoText   -> Video
    | AnswerText  -> Answer
    | PodcastText -> Podcast
    | _           -> Unknown

let contentTypeToId = function
    | ArticleText ->  0
    | VideoText   ->  1
    | AnswerText  ->  2
    | PodcastText ->  3
    | _           -> -1

let contentTypeToString = function
    | Article -> ArticleText
    | Video   -> VideoText  
    | Answer  -> AnswerText 
    | Podcast -> PodcastText
    | Unknown -> UnknownText   

let contentTypeIdToString = function
    | 0 -> ArticleText
    | 1 -> VideoText  
    | 2 -> AnswerText 
    | 3 -> PodcastText
    | _ -> UnknownText
    
[<CLIMutable>]
type ThumbnailResponse = {
    ImageUrl: string
    Platform: string
}

type Synched = { 
    Id:          int
    SourceId:    int
    LastSynched: DateTime
}
type User = { AccessId: string; ProfileId: string }

type PlatformUser = {
    ProfileId: string
    Platform:  Platform
    User:      User
    APIKey:    string
}

type LinkTopic = { Link:Link; Topic:Topic }

[<CLIMutable>]
type SubscribeActionResponse = { 
    User:     ProviderRequest
    Provider: ProviderRequest
}

[<CLIMutable>]
type Bootstrap = { 
    Providers: ProviderRequest list
    Platforms: String list
}

type Result<'a> = 
    | Success of 'a
    | Failure

type Command =
    | UpdateProfile   of ProfileRequest
    | UpdateThumbnail of UpdateThumbnailRequest

    | Follow          of FollowRequest
    | Unsubscribe     of UnsubscribeRequest
  
    | AddLink         of Link
    | RemoveLink      of RemoveLinkRequest
    | FeatureLink     of FeatureLinkRequest
    | ObserveLinks    of ObservedLinks
  
    | UpdateTopics    of FeaturedTopicsRequest
  
    | AddSource       of DataSourceRequest
    | RemoveSource    of RemoveDataSourceRequest
    | SyncSource      of DataSourceRequest

// FUNCTIONS
let isEmpty portfolio =
    portfolio.Articles |> List.isEmpty &&
    portfolio.Videos   |> List.isEmpty &&
    portfolio.Podcasts |> List.isEmpty &&
    portfolio.Answers  |> List.isEmpty