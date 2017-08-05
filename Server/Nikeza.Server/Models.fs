namespace Nikeza.Server.Models

open System

[<CLIMutable>]
type Profile = {
    ProfileId: int
    FirstName: string
    LastName: string
    Email: string
    ImageUrl: string
    Bio: string
    PasswordHash:string
    Created: DateTime
}

[<CLIMutable>]
type FollowRequest = { SubscriberId: int; ProviderId: int }

[<CLIMutable>]
type UnsubscribeRequest = { SubscriberId: int; ProviderId:   int }

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
type FeatureLinkRequest = { LinkId: int; IsFeatured: bool }

[<CLIMutable>]
type UpdateProfileRequest = {
    ProviderId: int
    Bio:        string
    Email:      string
}

[<CLIMutable>]
type LinksRequest = { ProviderId: int }

type Command =
    | Register      of Profile
    | Follow        of FollowRequest
    | Unsubscribe   of UnsubscribeRequest
    | AddLink       of AddLinkRequest
    | FeatureLink   of FeatureLinkRequest
    | UpdateProfile of UpdateProfileRequest

type Request =
    | GetLinks         of LinksRequest
    | GetFollowers     of Profile
    | GetSubscriptions of Profile