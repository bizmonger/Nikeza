module Domain.Core exposing (..)

import Controls.Login as Login exposing (Model)
import Http


type alias JsonProfile =
    { id : Int
    , firstName : String
    , lastName : String
    , email : String
    }


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


type Linksfrom
    = FromOther
    | FromPortal


type SubscriptionUpdate
    = Subscribe Id Id
    | Unsubscribe Id Id


type alias Provider =
    { profile : Profile
    , topics : List Topic
    , links : Links
    , recentLinks : List Link
    , subscriptions : Subscriptionsfunction
    , followers : Followersfunction
    }


type Subscribers
    = Subscribers (List Provider)


initSubscription : Id -> Subscribers
initSubscription profileId =
    Subscribers []


initProvider : Provider
initProvider =
    Provider initProfile initTopics initLinks [] initSubscription initSubscription


type alias Portal =
    { provider : Provider
    , sourcesNavigation : Bool
    , addLinkNavigation : Bool
    , linksNavigation : Bool
    , requested : ProviderRequest
    , newSource : Source
    , newLinks : NewLinks
    }


initPortal : Portal
initPortal =
    { provider = initProvider
    , sourcesNavigation = False
    , addLinkNavigation = False
    , linksNavigation = False
    , requested = EditProfile
    , newSource = initSource
    , newLinks = initNewLinks
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
    , bio = undefined
    , sources = []
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


type alias Topic =
    { name : String, isFeatured : Bool }


getTopic : Topic -> String
getTopic topic =
    topic.name


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
    , isFeatured : Bool
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
    , isFeatured = False
    }


initLinkToCreate : LinkToCreate
initLinkToCreate =
    { base = initLink
    , currentTopic = Topic "" False
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
    { platform : String, username : String, linksFound : Int }


initSource : Source
initSource =
    Source "" "" 0


type ProviderRequest
    = ViewSources
    | ViewLinks
    | AddLink
    | EditProfile
    | ViewSubscriptions
    | ViewFollowers
    | ViewProviders
    | ViewRecent



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


type alias Providerfunction =
    Id -> Maybe Provider


type alias Providersfunction =
    List Provider


type alias Loginfunction =
    Login.Model -> Login.Model


type alias Registerfunction msg =
    Form -> (Result Http.Error JsonProfile -> msg) -> Cmd msg



-- Form -> Result String Provider


type alias Linksfunction =
    Id -> Links


type alias TopicLinksfunction =
    Topic -> ContentType -> Id -> List Link


type alias UserNameToIdfunction =
    String -> Id


type alias SuggestedTopicsfunction =
    String -> List Topic


type alias Subscriptionsfunction =
    Id -> Subscribers


type alias Followersfunction =
    Id -> Subscribers


type alias Followfunction =
    Id -> Id -> Result String ()


type alias Unsubscribefunction =
    Id -> Id -> Result String ()


type ContentType
    = Article
    | Video
    | Podcast
    | Answer
    | All
    | Unknown



-- FUNCTIONS


compareLinks : Link -> Link -> Order
compareLinks a b =
    if a.isFeatured then
        LT
    else if b.isFeatured then
        GT
    else
        EQ


getPosts : ContentType -> Links -> List Link
getPosts contentType links =
    case contentType of
        Answer ->
            links.answers

        Article ->
            links.articles

        Podcast ->
            links.podcasts

        Video ->
            links.videos

        Unknown ->
            []

        All ->
            []


hasMatch : Topic -> List Topic -> Bool
hasMatch topic topics =
    topics |> toTopicNames |> List.member (getTopic topic)


linksExist : Links -> Bool
linksExist links =
    not <| links == initLinks


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


toUrl : Link -> String
toUrl link =
    getUrl link.url


toTopicNames : List Topic -> List String
toTopicNames topics =
    topics |> List.map (\topic -> topic.name)


providerTopicUrl : Maybe Id -> Id -> Topic -> Url
providerTopicUrl clientId providerId topic =
    case clientId of
        Just idOfRequestor ->
            Url <| "/#/portal/" ++ getId idOfRequestor ++ "/provider/" ++ getId providerId ++ "/" ++ getTopic topic

        Nothing ->
            Url <| "/#/provider/" ++ getId providerId ++ "/" ++ getTopic topic


providerUrl : Maybe Id -> Id -> Url
providerUrl clientId providerId =
    case clientId of
        Just idOfRequestor ->
            Url <| "/#/portal/" ++ getId idOfRequestor ++ "/provider/" ++ getId providerId

        Nothing ->
            Url <| "/#/provider/" ++ getId providerId


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


allContentUrl : Linksfrom -> Id -> ContentType -> Url
allContentUrl linksFrom id contentType =
    case linksFrom of
        FromOther ->
            Url <| "/#/provider/" ++ getId id ++ "/all/" ++ (contentType |> contentTypeToText)

        FromPortal ->
            Url <| "/#/portal/" ++ getId id ++ "/all/" ++ (contentType |> contentTypeToText)


allTopicContentUrl : Linksfrom -> Id -> ContentType -> Topic -> Url
allTopicContentUrl linksFrom id contentType topic =
    case linksFrom of
        FromOther ->
            Url <| "/#/provider/" ++ getId id ++ "/" ++ getTopic topic ++ "/all/" ++ (contentType |> contentTypeToText)

        FromPortal ->
            Url <| "/#/portal/" ++ getId id ++ "/" ++ getTopic topic ++ "/all/" ++ (contentType |> contentTypeToText)
