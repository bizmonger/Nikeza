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


getTopics : List Topic -> List String
getTopics topics =
    topics |> List.map (\t -> getTopic t)


type alias Post =
    { contributor : Profile
    , title : Title
    , url : Url
    , topics : List Topic
    }


type alias Contributorsfunction =
    List Profile


type alias Loginfunction =
    Login.Model -> Login.Model


type alias Contributorfunction =
    Id -> Maybe Profile


tryLogin : Loginfunction -> String -> String -> Login.Model
tryLogin loginf username password =
    loginf <| Login.Model username password False


type ContentType
    = Article
    | Video
    | Podcast
    | All


type alias LatestPostsfunction =
    Id -> ContentType -> List Post


type alias ContentTypefunction =
    ContentType -> Id -> List Post


type alias TopicPostsfunction =
    Topic -> ContentType -> Id -> List Post


latestPosts : LatestPostsfunction -> Id -> ContentType -> List Post
latestPosts f profileId contentType =
    f profileId contentType


getContent : ContentTypefunction -> Id -> ContentType -> List Post
getContent f profileId contentType =
    profileId |> f contentType


getPosts : TopicPostsfunction -> Topic -> ContentType -> Id -> List Post
getPosts topicPostsfunction topic contentType id =
    id |> topicPostsfunction topic contentType


undefined : String
undefined =
    "undefined"


topicUrl : Id -> Topic -> Url
topicUrl id topic =
    Url undefined


contributorTopicUrl : Id -> Topic -> Url
contributorTopicUrl id topic =
    Url <| "/#/contributor/" ++ getId id ++ "/" ++ getTopic topic


contributorUrl : Id -> Url
contributorUrl id =
    Url <| "/#/contributor/" ++ getId id


moreContributorContentUrl : Id -> ContentType -> Url
moreContributorContentUrl id contentType =
    let
        toText contentType =
            case contentType of
                Article ->
                    "articles"

                Video ->
                    "videos"

                Podcast ->
                    "podcasts"

                All ->
                    ""
    in
        Url <| "/#/contributor/" ++ getId id ++ "/all/" ++ (contentType |> toText)
