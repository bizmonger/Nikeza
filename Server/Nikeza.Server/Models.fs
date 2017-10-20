module Nikeza.Server.Model

open System
open Newtonsoft.Json

type ContentType = 
    | Article
    | Video
    | Answer
    | Podcast
    | Unknown

type PlatformType =
    | YouTube
    | WordPress
    | StackOverflow
    | Other

type RawContentType = string

let contentTypeFromString = function
    | "article" -> Article
    | "video"   -> Video
    | "answer"  -> Answer
    | "podcast" -> Podcast
    | _         -> Unknown

let contentTypeToId = function
    | "article" ->  0
    | "video"   ->  1
    | "answer"  ->  2
    | "podcast" ->  3
    | _         -> -1

let contentTypeToString = function
    | Article -> "article"
    | Video   -> "video"  
    | Answer  -> "answer" 
    | Podcast -> "podcast"
    | Unknown -> "unknown"    

let contentTypeIdToString = function
    | 0 -> "article"
    | 1 -> "video"  
    | 2 -> "answer" 
    | 3 -> "podcast"
    | _ -> "unknown"    

[<CLIMutable>]
type Profile = {
    ProfileId:    string
    FirstName:    string
    LastName:     string
    Email:        string
    ImageUrl:     string
    Bio:          string
    PasswordHash: string
    Sources:      string list
    Salt:         string
    Created:      DateTime
}

[<CLIMutable>]
type FollowRequest =      { SubscriberId: string; ProfileId: string }

[<CLIMutable>]
type UnsubscribeRequest = { SubscriberId: string; ProfileId: string }

[<CLIMutable>]
type RemoveLinkRequest =  { LinkId: int }

[<CLIMutable>]
type AddLinkRequest = { 
    ProfileId:    string
    Title:         String
    Description:   String
    Url:           string
    IsFeatured:    bool
    ContentType:   string
}

[<CLIMutable>]
[<JsonObject>]
type Topic = { 
    Id:         int
    Name:       string
    IsFeatured: bool 
}

[<CLIMutable>]
[<JsonObject>]
type Link = { 
    Id:            int
    ProfileId:     string
    Title:         String
    Url:           string
    Topics:        Topic list
    ContentType:   string
    IsFeatured:    bool
}

[<CLIMutable>]
type FeatureLinkRequest = { LinkId: int; IsFeatured: bool }

type PlatformUsername = {
    Platform:  PlatformType
    Username:  string
}

[<CLIMutable>]
[<JsonObject>]
type SourceRequest = { 
    Id:        int
    ProfileId: string
    Platform:  string
    Username:  string
    Links:     Link list
}

[<CLIMutable>]
type RemoveSourceRequest = { Id: int }

[<CLIMutable>]
type ProfileRequest = {
    ProfileId:  string
    FirstName:  string
    LastName:   string
    Bio:        string
    Email:      string
    ImageUrl:   string
    Sources:    string list
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

    | AddLink       of AddLinkRequest
    | RemoveLink    of RemoveLinkRequest
    | FeatureLink   of FeatureLinkRequest

    | AddSource     of SourceRequest
    | RemoveSource  of RemoveSourceRequest