module Domain.Core exposing (..)

import Controls.Login as Login exposing (Model)


type alias Profile =
    { id : Id
    , name : Contributor
    , imageUrl : Url
    , bio : String
    , topics : List Topic
    }


type Id
    = Id String


getId : Id -> String
getId id =
    let
        (Id value) =
            id
    in
        value


type Contributor
    = Contributor String


getName : Contributor -> String
getName contributor =
    let
        (Contributor value) =
            contributor
    in
        value


type Title
    = Title String


type Url
    = Url String


getUrl : Url -> String
getUrl url =
    let
        (Url value) =
            url
    in
        value


type Topic
    = Topic String


gettopic : Topic -> String
gettopic topic =
    let
        (Topic value) =
            topic
    in
        value


type Video
    = Video Post


type Article
    = Article Post


type Podcast
    = Podcast Post


type alias Post =
    { contributor : Profile, title : Title, url : Url }


type alias ContributorUrlfunction =
    Id -> Url


type alias Contributorsfunction =
    List Profile


type alias Loginfunction =
    Login.Model -> Login.Model


type alias GetContributorfunction =
    Id -> Maybe Profile


tryLogin : Loginfunction -> String -> String -> Login.Model
tryLogin loginf username password =
    loginf <| Login.Model username password False


type alias TopicUrlfunction =
    Id -> Topic -> Url


topicUrl : TopicUrlfunction -> Id -> Topic -> Url
topicUrl f id topic =
    f id topic


type ContentType
    = Articles
    | Videos
    | Podcasts


type alias LatestPostsfunction =
    Id -> ContentType -> List Post


type alias Videosfunction =
    Id -> List Video


type alias Articlesfunction =
    Id -> List Article


type alias Podcastsfunction =
    Id -> List Podcast


latestPosts : LatestPostsfunction -> Id -> ContentType -> List Post
latestPosts f profileId contentType =
    f profileId contentType


videos : Videosfunction -> Id -> List Video
videos f profileId =
    f profileId


articles : Articlesfunction -> Id -> List Article
articles f profileId =
    f profileId


podcasts : Podcastsfunction -> Id -> List Podcast
podcasts f profileId =
    f profileId


undefined : String
undefined =
    "undefined"
