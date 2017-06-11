module Domain.Core exposing (..)

import Controls.Login as Login exposing (Model)


-- Types


type alias Profile =
    { id : Id
    , name : Name
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


type Name
    = Name String


getName : Name -> String
getName name =
    let
        (Name value) =
            name
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


type alias Link =
    { getName : Profile
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


type alias LatestLinksfunction =
    Id -> ContentType -> List Link


type alias ContentTypefunction =
    ContentType -> Id -> List Link


type alias TopicLinksfunction =
    Topic -> ContentType -> Id -> List Link


type alias UserNameToIdfunction =
    String -> Id


type ContentType
    = Article
    | Video
    | Podcast
    | Answer
    | All



-- Functions


tryLogin : Loginfunction -> String -> String -> Login.Model
tryLogin loginf username password =
    loginf <| Login.Model username password False


latestLinks : LatestLinksfunction -> Id -> ContentType -> List Link
latestLinks f profileId contentType =
    f profileId contentType


getContent : ContentTypefunction -> Id -> ContentType -> List Link
getContent f profileId contentType =
    profileId |> f contentType


getLinks : TopicLinksfunction -> Topic -> ContentType -> Id -> List Link
getLinks topicLinksfunction topic contentType id =
    id |> topicLinksfunction topic contentType


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


toContentType : String -> ContentType
toContentType contentType =
    case contentType of
        "Articles" ->
            Article

        "Videos" ->
            Video

        "Podcasts" ->
            Podcast

        "Answers" ->
            Podcast

        _ ->
            All


contentTypeToText : ContentType -> String
contentTypeToText contentType =
    case contentType of
        Article ->
            "Articles"

        Video ->
            "Videos"

        Podcast ->
            "Podcasts"

        Answer ->
            "Answers"

        All ->
            ""


moreContributorContentUrl : Id -> ContentType -> Url
moreContributorContentUrl id contentType =
    Url <| "/#/contributor/" ++ getId id ++ "/all/" ++ (contentType |> contentTypeToText)


moreContributorContentOnTopicUrl : Id -> ContentType -> Topic -> Url
moreContributorContentOnTopicUrl id contentType topic =
    Url <| "/#/contributor/" ++ getId id ++ "/" ++ getTopic topic ++ "/all/" ++ (contentType |> contentTypeToText)
