module Nikeza.Server.Model

open System
open Literals

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
type RegistrationRequest = {
        FirstName: string 
        LastName:  string
        Email:     string
        Password:  string
    }
        
[<CLIMutable>]
type LogInRequest = {
        Email:    string
        Password: string 
    }

[<CLIMutable>]
type FollowRequest =      { SubscriberId: string; ProfileId: string }

[<CLIMutable>]
type UnsubscribeRequest = { SubscriberId: string; ProfileId: string }

[<CLIMutable>]
type RemoveLinkRequest =  { LinkId: int }

[<CLIMutable>]
type UpdateThumbnailRequest = {
    ProfileId: string
    ImageUrl:  string
}

[<CLIMutable>]
type ThumbnailResponse = {
    ImageUrl: string
    Platform: string
}

[<CLIMutable>]
type Topic = { 
    Id:   int
    Name: string
}

[<CLIMutable>]
type ProviderTopic = { 
    Id:         int
    Name:       string
    IsFeatured: bool
}

[<CLIMutable>]
type Link = { 
    Id:            int
    ProfileId:     string
    Title:         String
    Description:   String
    Url:           string
    Topics:        ProviderTopic list
    ContentType:   string
    IsFeatured:    bool
}

[<CLIMutable>]
type FeatureLinkRequest = { LinkId: int; IsFeatured: bool }

[<CLIMutable>]
type RecentRequest = { SubscriberId: string }

type User = { AccessId: string; ProfileId: string }

type PlatformUser = {
    ProfileId: string
    Platform:  Platform
    User:      User
    APIKey:    string
}

[<CLIMutable>]
type ProviderTopicRequest = {
    ProfileId:  string
    TopicId:    int
    Name:       string
    IsFeatured: bool
}

type FeaturedTopicsRequest = {
    ProfileId:  string
    Names:      string list
}

[<CLIMutable>]
type DataSourceRequest = { 
    Id:        int
    ProfileId: string
    Platform:  string
    AccessId:  string
    Links:     Link seq
}

[<CLIMutable>]
type TopicRequest = { Name:string }

type LinkTopic = { Link:Link; Topic:Topic }

[<CLIMutable>]
type RemoveDataSourceRequest = { Id: int }

[<CLIMutable>]
type ObservedLinks = { SubscriberId: string; LinkIds: int list }

[<CLIMutable>]
type Profile = {
    Id:    string
    FirstName:    string
    LastName:     string
    Email:        string
    ImageUrl:     string
    Bio:          string
    PasswordHash: string
    Sources:      DataSourceRequest list
    Salt:         string
    Created:      DateTime
}

[<CLIMutable>]
type ProfileRequest = {
    Id:         string
    FirstName:  string
    LastName:   string
    Bio:        string
    Email:      string
    ImageUrl:   string
    Sources:    DataSourceRequest list
}

[<CLIMutable>]
type ProfileAndTopicsRequest ={
    Profile: ProfileRequest
    Topics:  ProviderTopic list
}

[<CLIMutable>]
type Portfolio = { 
    Answers : Link list
    Articles: Link list
    Videos:   Link list
    Podcasts: Link list
}

[<CLIMutable>]
type ProviderRequest = {
    Profile:       ProfileRequest
    Topics:        ProviderTopic  list
    Portfolio:     Portfolio
    RecentLinks:   Link           list
    Subscriptions: string list
    Followers:     string list
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

// FUNCTIONS
let isEmpty portfolio =
    portfolio.Articles |> List.isEmpty &&
    portfolio.Videos   |> List.isEmpty &&
    portfolio.Podcasts |> List.isEmpty &&
    portfolio.Answers  |> List.isEmpty