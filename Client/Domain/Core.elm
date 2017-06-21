module Domain.Core exposing (..)

import Controls.Login as Login exposing (Model)
import Controls.Register as Register exposing (Model)


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


type alias ContentProvider =
    { profile : Profile
    , showAll : Bool
    , topics : List Topic
    , links : Links
    }


initContentProvider : ContentProvider
initContentProvider =
    let
        addedLinks =
            NewLinks initLinkToCreate False []
    in
        ContentProvider initProfile True initTopics initLinks


type alias Portal =
    { contentProvider : ContentProvider
    , profileCompleted : Bool
    , requested : ContentProviderRequest
    , newSource : Source
    , newLinks : NewLinks
    }


initPortal : Portal
initPortal =
    { contentProvider = initContentProvider
    , profileCompleted = False
    , requested = EditProfile
    , newSource = initSource
    , newLinks = initNewLinks
    }


type alias Profile =
    { id : Id
    , name : Name
    , email : Email
    , imageUrl : Url
    , bio : String
    , connections : List Source
    }


initProfile : Profile
initProfile =
    { id = Id undefined
    , name = Name undefined
    , email = Email undefined
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


type Email
    = Email String


getEmail : Email -> String
getEmail email =
    let
        (Email value) =
            email
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


type alias Source =
    { platform : String, username : String }


initSource : Source
initSource =
    { platform = "", username = "" }


type ContentProviderRequest
    = ViewSources
    | ViewLinks
    | AddLink
    | EditProfile



-- INTERFACES


type alias Sourcesfunction =
    Id -> List Source


type alias AddSourcefunction =
    Id -> Source -> Result String (List Source)


type alias RemoveSourcefunction =
    Id -> Source -> Result String (List Source)


type alias AddLinkfunction =
    Id -> Link -> Result String Links


type alias RemoveLinkfunction =
    Id -> Link -> Result String Links


type alias ContentProviderfunction =
    Id -> Maybe ContentProvider


type alias ContentProvidersfunction =
    List ContentProvider


type alias Loginfunction =
    Login.Model -> Login.Model


type alias Registerfunction =
    Register.Model -> Result String ContentProvider


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


contentProviderTopicUrl : Id -> Topic -> Url
contentProviderTopicUrl id topic =
    Url <| "/#/contentProvider/" ++ getId id ++ "/" ++ getTopic topic


contentProviderUrl : Id -> Url
contentProviderUrl id =
    Url <| "/#/contentProvider/" ++ getId id


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


moreContentProviderContentUrl : Id -> ContentType -> Url
moreContentProviderContentUrl id contentType =
    Url <| "/#/contentProvider/" ++ getId id ++ "/all/" ++ (contentType |> contentTypeToText)


moreContentProviderContentOnTopicUrl : Id -> ContentType -> Topic -> Url
moreContentProviderContentOnTopicUrl id contentType topic =
    Url <| "/#/contentProvider/" ++ getId id ++ "/" ++ getTopic topic ++ "/all/" ++ (contentType |> contentTypeToText)
