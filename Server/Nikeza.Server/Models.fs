module Nikeza.Server.Model

open System

type ContentType = 
    | Article
    | Video
    | Answer
    | Podcast
    | Unknown

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
    ProfileId:    int
    FirstName:    string
    LastName:     string
    Email:        string
    ImageUrl:     string
    Bio:          string
    PasswordHash: string
    Salt:         string
    Created:      DateTime
}

[<CLIMutable>]
type FollowRequest =      { SubscriberId: int; ProviderId: int }

[<CLIMutable>]
type UnsubscribeRequest = { SubscriberId: int; ProviderId: int }

[<CLIMutable>]
type RemoveLinkRequest =  { LinkId: int }

[<CLIMutable>]
type AddLinkRequest = { 
    ProviderId:    int
    Title:         String
    Description:   String
    Url:           string
    IsFeatured:    bool
    ContentType:   string
}

[<CLIMutable>]
type Link = { 
    Id:            int
    ProviderId:    int
    Title:         String
    Description:   String
    Url:           string
    IsFeatured:    bool
    ContentType:   string
}

[<CLIMutable>]
type FeatureLinkRequest = { LinkId: int; IsFeatured: bool }

[<CLIMutable>]
type AddSourceRequest = { 
    ProfileId: int
    Platform:  string
    Username:  string
}

[<CLIMutable>]
type RemoveSourceRequest = { SourceId: int }

[<CLIMutable>]
type ProfileRequest = {
    ProfileId: int
    FirstName:  string
    LastName:   string
    Bio:        string
    Email:      string
    ImageUrl:   string
}

type Command =
    | Register      of Profile
    | UpdateProfile of ProfileRequest

    | Follow        of FollowRequest
    | Unsubscribe   of UnsubscribeRequest  

    | AddLink       of AddLinkRequest
    | RemoveLink    of RemoveLinkRequest
    | FeatureLink   of FeatureLinkRequest

    | AddSource     of AddSourceRequest
    | RemoveSource  of RemoveSourceRequest