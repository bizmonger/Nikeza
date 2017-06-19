module Domain.Core exposing (..)

import Controls.Login as Login exposing (Model)


-- TYPES


initTopics : List Topic
initTopics =
    []


type alias Links =
    { answers : List Link
    , articles : List Link
    , videos : List Link
    , podcasts : List Link
    }


initLinks : Links
initLinks =
    { answers = []
    , articles = []
    , videos = []
    , podcasts = []
    }


type alias Contributor =
    { profile : Profile
    , showAll : Bool
    , topics : List Topic
    , links : Links
    }


initContributor : Contributor
initContributor =
    let
        addedLinks =
            NewLinks initLinkToCreate False []
    in
        Contributor initProfile True initTopics initLinks


type alias ContributorPortal =
    { contributor : Contributor
    , requested : ContributorRequest
    , newConnection : Connection
    , newLinks : NewLinks
    }


type alias Profile =
    { id : Id
    , name : Name
    , imageUrl : Url
    , bio : String
    , connections : List Connection
    }


initProfile : Profile
initProfile =
    { id = Id undefined
    , name = Name undefined
    , imageUrl = Url undefined
    , bio = undefined
    , connections = []
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


type Platform
    = Platform String


getPlatform : Platform -> String
getPlatform platform =
    let
        (Platform value) =
            platform
    in
        value


type alias Link =
    { profile : Profile
    , title : Title
    , url : Url
    , contentType : ContentType
    , topics : List Topic
    }


type alias LinkToCreate =
    { base : Link, currentTopic : Topic }


initLink : Link
initLink =
    { profile = initProfile
    , title = Title ""
    , url = Url ""
    , contentType = Unknown
    , topics = []
    }


initLinkToCreate : LinkToCreate
initLinkToCreate =
    { base = initLink
    , currentTopic = Topic ""
    }


type alias NewLinks =
    { current : LinkToCreate
    , canAdd : Bool
    , added : List Link
    }


initNewLinks : NewLinks
initNewLinks =
    { current = initLinkToCreate, canAdd = False, added = [] }


type alias Connection =
    { platform : String, username : String }


initConnection : Connection
initConnection =
    { platform = "", username = "" }


type ContributorRequest
    = ViewConnections
    | ViewLinks
    | AddLink



-- INTERFACES


type alias Connectionsfunction =
    Id -> List Connection


type alias Contributorfunction =
    Id -> Maybe Contributor


type alias Contributorsfunction =
    List Contributor


type alias Loginfunction =
    Login.Model -> Login.Model


type alias Linksfunction =
    Id -> Links


type alias TopicLinksfunction =
    Topic -> ContentType -> Id -> List Link


type alias UserNameToIdfunction =
    String -> Id


type alias SuggestedTopicsfunction =
    String -> List Topic


type ContentType
    = Article
    | Video
    | Podcast
    | Answer
    | All
    | Unknown



-- FUNCTIONS


tryLogin : Loginfunction -> String -> String -> Login.Model
tryLogin loginf username password =
    loginf <| Login.Model username password False


getContent : Linksfunction -> Id -> Links
getContent f profileId =
    profileId |> f


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

        "Article" ->
            Article

        "Videos" ->
            Video

        "Video" ->
            Video

        "Podcasts" ->
            Podcast

        "Podcast" ->
            Podcast

        "Answers" ->
            Answer

        "Answer" ->
            Answer

        "Unknown" ->
            Unknown

        _ ->
            Unknown


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

        Unknown ->
            "Unknown"

        All ->
            ""


moreContributorContentUrl : Id -> ContentType -> Url
moreContributorContentUrl id contentType =
    Url <| "/#/contributor/" ++ getId id ++ "/all/" ++ (contentType |> contentTypeToText)


moreContributorContentOnTopicUrl : Id -> ContentType -> Topic -> Url
moreContributorContentOnTopicUrl id contentType topic =
    Url <| "/#/contributor/" ++ getId id ++ "/" ++ getTopic topic ++ "/all/" ++ (contentType |> contentTypeToText)
