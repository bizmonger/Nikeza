namespace Nikeza.Server.Models

open System

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
type RemoveLinkRequest = { LinkId: int }

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