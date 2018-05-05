module Nikeza.Common

open System

type nonempty<'a> = {
    Head : 'a
    Tail : 'a list
}

type Id =          Id         of string
type ProviderId =  ProviderId of string
type ProfileId =   ProfileId  of string
type LinkId =      LinkId     of int
type TopicId =     TopicId    of int
type Url = Url                of string

type Platform = Platform of string

type error<'a> = { Context:'a; Description:string }

[<CLIMutable>]
type LogInRequest = {
    Email:    string
    Password: string 
}

[<CLIMutable>]
type RegistrationRequest = {
    FirstName: string 
    LastName:  string
    Email:     string
    Password:  string
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
    ; Title:         string
    Description:   string
    Url:           string
    Topics:        ProviderTopic list
    ContentType:   string
    IsFeatured:    bool
    Timestamp:     DateTime
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
type FeatureLinkRequest = { LinkId: int; IsFeatured: bool }

type FeatureTopicsrequest = { 
    ProfileId: Id
    Names:     string list
}

[<CLIMutable>]
type RecentRequest = { SubscriberId: string }

[<CLIMutable>]
type DataSourceRequest = { 
    Id:        int
    ProfileId: string
    Platform:  string
    AccessId:  string
    Links:     Link seq
}

type DataSource = DataSourceRequest

[<CLIMutable>]
type TopicRequest = { Name:string }

[<CLIMutable>]
type RemoveDataSourceRequest = { Id: int }

[<CLIMutable>]
type ObservedLinks = { SubscriberId: string; LinkIds: int list }

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
type Portfolio = { 
    Answers : Link list
    Articles: Link list
    Videos:   Link list
    Podcasts: Link list
}

[<CLIMutable>]
type ProviderRequest = {
    Profile:       ProfileRequest
    Topics:        ProviderTopic list
    Portfolio:     Portfolio
    RecentLinks:   Link   list
    Subscriptions: string list
    Followers:     string list
}

type FeaturedTopicsRequest = {
    ProfileId:  string
    Names:      string list
}

[<CLIMutable>]
type Profile = {
    Id:           string
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
type ProviderTopicRequest = {
    ProfileId:  string
    TopicId:    int
    Name:       string
    IsFeatured: bool
}

[<CLIMutable>]
type ProfileAndTopicsRequest ={
    Profile: ProfileRequest
    Topics:  ProviderTopic list
}

let uninitializedProfile:ProfileRequest = { 
    Id =        ""
    FirstName = ""
    LastName =  ""
    Email =     ""
    ImageUrl =  ""
    Bio =       ""
    Sources =   []
}

let uninitializedPortfolio = { 
    Answers  = []
    Articles = []
    Videos =   []
    Podcasts = []
}

let uninitializedProvider = {
    Profile =      uninitializedProfile
    Topics =        []
    Portfolio=     uninitializedPortfolio
    RecentLinks=   []
    Subscriptions= []
    Followers=     []
}

let handle' event handlers= 
    handlers|> List.iter(fun handle -> handle event)

let handle event handlers= 
     
    handlers.Head::handlers.Tail 
     |> List.iter(fun handle -> handle event)