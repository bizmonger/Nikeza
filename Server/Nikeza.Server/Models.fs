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
    Created: DateTime
}

[<CLIMutable>]
type FollowRequest = {
    SubscriberId: int 
    ProviderId:   int
}

[<CLIMutable>]
type UnsubscribeRequest = {
    SubscriberId: int 
    ProviderId:   int 
}

[<CLIMutable>]
type FeatureLinkRequest = {
    LinkId:  int
    Enabled: bool
}

type Command =
    | Follow      of FollowRequest
    | Unsubscribe of UnsubscribeRequest
    | FeatureLink of FeatureLinkRequest