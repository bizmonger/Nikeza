module Nikeza.Server.Model

open System
open Nikeza.Server.Literals

type ContentType = 
    | Article
    | Video
    | Answer
    | Podcast
    | Unknown

type Platform =
    | YouTube
    | WordPress
    | StackOverflow
    | Other

let PlatformToString = function
    | YouTube       -> "youtube"
    | WordPress     -> "wordpress"
    | StackOverflow -> "stackoverflow"
    | Other         -> "other"

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
type FollowRequest =      { SubscriberId: string; ProfileId: string }

[<CLIMutable>]
type UnsubscribeRequest = { SubscriberId: string; ProfileId: string }

[<CLIMutable>]
type RemoveLinkRequest =  { LinkId: int }

[<CLIMutable>]
type Topic = { 
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
    Topics:        Topic list
    ContentType:   string
    IsFeatured:    bool
}

[<CLIMutable>]
type FeatureLinkRequest = { LinkId: int; IsFeatured: bool }

type User = { AccessId: string; ProfileId: string }

type PlatformUser = {
    ProfileId: string
    Platform:  Platform
    User:      User
    APIKey:    string
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
type RemoveDataSourceRequest = { Id: int }

[<CLIMutable>]
type Profile = {
    ProfileId:    string
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
    ProfileId:  string
    FirstName:  string
    LastName:   string
    Bio:        string
    Email:      string
    ImageUrl:   string
    Sources:    DataSourceRequest list
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
    Topics:        Topic     list
    Portfolio:     Portfolio
    Subscriptions: string    list
    Followers:     string    list
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
    | Register      of Profile
    | UpdateProfile of ProfileRequest

    | Follow        of FollowRequest
    | Unsubscribe   of UnsubscribeRequest  

    | AddLink       of Link
    | RemoveLink    of RemoveLinkRequest
    | FeatureLink   of FeatureLinkRequest

    | AddSource     of DataSourceRequest
    | RemoveSource  of RemoveDataSourceRequest