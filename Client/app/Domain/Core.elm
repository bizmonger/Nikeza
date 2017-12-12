module Domain.Core exposing (..)

import Html.Events exposing (on, keyCode, onInput)
import List.Extra exposing (uniqueBy)
import Dict
import Dict.Extra as Dict
import Json.Decode as Json
import Html exposing (..)


initForm : Form
initForm =
    Form "" "" "" "" ""


type alias Form =
    { firstName : String
    , lastName : String
    , email : String
    , password : String
    , confirm : String
    }


type alias Credentials =
    { email : String
    , password : String
    , loggedIn : Bool
    }


initCredentials : Credentials
initCredentials =
    Credentials "" "" False


initTopics : List Topic
initTopics =
    []


type alias PortfolioSearch =
    { provider : Provider
    , topicSuggestions : List Topic
    , selectedTopic : Topic
    , inputValue : String
    }


type alias Portfolio =
    { topics : List Topic
    , answers : List Link
    , articles : List Link
    , videos : List Link
    , podcasts : List Link
    }


initPortfolio : Portfolio
initPortfolio =
    { topics = []
    , answers = []
    , articles = []
    , videos = []
    , podcasts = []
    }


type Linksfrom
    = FromOther
    | FromPortal


type SubscriptionUpdate
    = Subscribe Provider Provider
    | Unsubscribe Provider Provider


type alias Provider =
    { profile : Profile
    , topics : List Topic
    , portfolio : Portfolio
    , filteredPortfolio : Portfolio
    , recentLinks : List Link
    , followers : List Id
    , subscriptions : List Id
    }


type alias ProfileEditor =
    { provider : Provider
    , currentTopic : Topic
    , chosenTopics : List Topic
    , topicSuggestions : List Topic
    }


initProfileEditor : ProfileEditor
initProfileEditor =
    { provider = initProvider
    , currentTopic = initTopic
    , chosenTopics = initTopics
    , topicSuggestions = initTopics
    }


type Members
    = Members (List Provider)


initTopic : Topic
initTopic =
    { name = "", isFeatured = False }


initPortfolioSearch : PortfolioSearch
initPortfolioSearch =
    { provider = initProvider, topicSuggestions = [], selectedTopic = initTopic, inputValue = "" }


initSubscription : List Id
initSubscription =
    []


initProvider : Provider
initProvider =
    Provider initProfile initTopics initPortfolio initPortfolio [] initSubscription initSubscription


type alias Portal =
    { provider : Provider
    , sourcesNavigation : Bool
    , addLinkNavigation : Bool
    , portfolioNavigation : Bool
    , requested : ProviderRequest
    , newSource : Source
    , newLinks : NewLinks
    , profileEditor : ProfileEditor
    }


initPortal : Portal
initPortal =
    { provider = initProvider
    , sourcesNavigation = False
    , addLinkNavigation = False
    , portfolioNavigation = False
    , requested = EditProfile
    , newSource = initSource
    , newLinks = initNewLinks
    , profileEditor = initProfileEditor
    }


type ProviderLinks
    = ProviderLinks LinkFields


type alias LinkFields =
    { links : List Link
    }


type alias Profile =
    { id : Id
    , firstName : Name
    , lastName : Name
    , email : Email
    , imageUrl : Url
    , bio : String
    , sources : List Source
    }


initProfile : Profile
initProfile =
    { id = Id undefined
    , firstName = Name undefined
    , lastName = Name undefined
    , email = Email undefined
    , imageUrl = Url undefined
    , bio = ""
    , sources = []
    }


type Id
    = Id String


idText : Id -> String
idText id =
    let
        (Id value) =
            id
    in
        value


type Name
    = Name String


nameText : Name -> String
nameText name =
    let
        (Name value) =
            name
    in
        value


type Email
    = Email String


emailText : Email -> String
emailText email =
    let
        (Email value) =
            email
    in
        value


type Title
    = Title String


titleText : Title -> String
titleText title =
    let
        (Title value) =
            title
    in
        value


type Url
    = Url String


urlText : Url -> String
urlText url =
    let
        (Url value) =
            url
    in
        value


type alias Topic =
    { name : String, isFeatured : Bool }


topicText : Topic -> String
topicText topic =
    topic.name


type Platform
    = Platform String


platformText : Platform -> String
platformText platform =
    let
        (Platform value) =
            platform
    in
        value


type alias UpdateThumbnailRequest =
    { profileId : Id
    , imageUrl : Url
    }


type alias Link =
    { id : Int
    , profileId : Id
    , title : Title
    , url : Url
    , topics : List Topic
    , contentType : ContentType
    , isFeatured : Bool
    , timestamp : String
    }


type alias LinkToCreate =
    { base : Link, currentTopic : Topic, topicSuggestions : List Topic }


initLink : Link
initLink =
    { id = -1
    , profileId = Id undefined
    , title = Title ""
    , url = Url ""
    , contentType = Unknown
    , topics = []
    , isFeatured = False
    , timestamp = "1/1/1900"
    }


initLinkToCreate : LinkToCreate
initLinkToCreate =
    { base = initLink
    , currentTopic = Topic "" False
    , topicSuggestions = []
    }


type alias NewLinks =
    { profileId : Id
    , current : LinkToCreate
    , canAdd : Bool
    , added : List Link
    }


initNewLinks : NewLinks
initNewLinks =
    { profileId = Id undefined, current = initLinkToCreate, canAdd = False, added = [] }


type alias SubscriptionRequest =
    { subscriberId : Id
    , providerId : Id
    }


type alias Source =
    { id : Id
    , profileId : Id
    , platform : String
    , accessId : String
    , links : List Link
    }


initSource : Source
initSource =
    Source (Id undefined) (Id undefined) "" "" []


type ProviderRequest
    = ViewSources
    | ViewPortfolio
    | AddLink
    | EditProfile
    | ViewSubscriptions
    | ViewFollowers
    | ViewProviders
    | ViewRecent



-- INTERFACES


type alias UserNameToIdfunction =
    String -> Id


type ContentType
    = Article
    | Video
    | Podcast
    | Answer
    | All
    | Featured
    | Unknown



-- FUNCTIONS


maxTopicsToShow : Int
maxTopicsToShow =
    8


topicGroups : List Topic -> List Topic
topicGroups someTopics =
    Dict.groupBy .name someTopics
        |> Dict.toList
        |> List.map (\( name, topicList ) -> ( name, List.length topicList ))
        |> List.sortBy (\( _, topicList ) -> topicList)
        |> List.reverse
        |> List.map (\t -> { name = t |> Tuple.first, isFeatured = True })
        |> List.take maxTopicsToShow


compareLinks : Link -> Link -> Order
compareLinks a b =
    if a.isFeatured then
        LT
    else if b.isFeatured then
        GT
    else
        EQ


getLinks : ContentType -> Portfolio -> List Link
getLinks contentType portfolio =
    case contentType of
        Answer ->
            portfolio.answers

        Article ->
            portfolio.articles

        Podcast ->
            portfolio.podcasts

        Video ->
            portfolio.videos

        All ->
            portfolio.answers
                ++ portfolio.articles
                ++ portfolio.podcasts
                ++ portfolio.videos

        Featured ->
            portfolio.answers
                ++ portfolio.articles
                ++ portfolio.podcasts
                ++ portfolio.videos
                |> List.filter .isFeatured

        Unknown ->
            []


topicsFromLinks : List Link -> List Topic
topicsFromLinks links =
    links
        |> List.map (\l -> l.topics)
        |> List.concat
        |> List.sortBy .name
        |> uniqueBy toString


hasMatch : Topic -> List Topic -> Bool
hasMatch topic topics =
    topics |> toTopicNames |> List.member (topicText topic)


portfolioExists : Portfolio -> Bool
portfolioExists portfolio =
    not <| portfolio == initPortfolio


undefined : String
undefined =
    "undefined"


topicUrl : Id -> Topic -> Url
topicUrl id topic =
    Url undefined


toUrl : Link -> String
toUrl link =
    urlText link.url


toTopicNames : List Topic -> List String
toTopicNames topics =
    topics |> List.map .name


providerTopicUrl : Maybe Id -> Id -> Topic -> Url
providerTopicUrl loggedIn providerId topic =
    case loggedIn of
        Just userId ->
            Url <| "/#/portal/" ++ idText userId ++ "/provider/" ++ idText providerId ++ "/" ++ topicText topic

        Nothing ->
            Url <| "/#/provider/" ++ idText providerId ++ "/" ++ topicText topic


providerUrl : Maybe Id -> Id -> Url
providerUrl loggedIn providerId =
    case loggedIn of
        Just userId ->
            Url <| "/#/portal/" ++ idText userId ++ "/provider/" ++ idText providerId

        Nothing ->
            Url <| "/#/provider/" ++ idText providerId


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

        All ->
            "Content"

        Featured ->
            "Featured"

        Unknown ->
            "Unknown"


allContentUrl : Linksfrom -> Id -> ContentType -> Url
allContentUrl linksFrom id contentType =
    case linksFrom of
        FromOther ->
            Url <| "/#/provider/" ++ idText id ++ "/all/" ++ (contentType |> contentTypeToText)

        FromPortal ->
            Url <| "/#/portal/" ++ idText id ++ "/all/" ++ (contentType |> contentTypeToText)


title : Topic -> ContentType -> String
title topic contentType =
    if contentType == All then
        topic.name
    else
        contentType |> contentTypeToText


allTopicContentUrl : Linksfrom -> Id -> ContentType -> Topic -> Url
allTopicContentUrl linksFrom id contentType topic =
    case linksFrom of
        FromOther ->
            Url <| "/#/provider/" ++ idText id ++ "/" ++ topicText topic ++ "/all/" ++ title topic contentType

        FromPortal ->
            Url <| "/#/portal/" ++ idText id ++ "/" ++ topicText topic ++ "/all/" ++ title topic contentType


onKeyDown : (Int -> msg) -> Attribute msg
onKeyDown tagger =
    on "keydown" (Json.map tagger keyCode)
