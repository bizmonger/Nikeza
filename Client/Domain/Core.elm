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


getTitle : Title -> String
getTitle title =
    let
        (Title value) =
            title
    in
        value


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


getTopic : Topic -> String
getTopic topic =
    let
        (Topic value) =
            topic
    in
        value


type alias Post =
    { contributor : Profile, title : Title, url : Url, topics : List Topic }


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
    = Article
    | Video
    | Podcast


type alias LatestPostsfunction =
    Id -> ContentType -> List Post


type alias ContentTypefunction =
    Id -> ContentType -> List Post


latestPosts : LatestPostsfunction -> Id -> ContentType -> List Post
latestPosts f profileId contentType =
    f profileId contentType


getContent : ContentTypefunction -> Id -> ContentType -> List Post
getContent f profileId contentType =
    f profileId contentType


undefined : String
undefined =
    "undefined"
