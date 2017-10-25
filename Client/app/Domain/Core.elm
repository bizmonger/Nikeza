module Domain.Core exposing (..)


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


initTopics : List Topic
initTopics =
    []


type alias Portfolio =
    { answers : List Link
    , articles : List Link
    , videos : List Link
    , podcasts : List Link
    }


initPortfolio : Portfolio
initPortfolio =
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
    , portfolio : Portfolio
    , filteredPortfolio : Portfolio
    , recentLinks : List Link
    , followers : Members
    , subscriptions : Members
    }


type Members
    = Members (List Provider)


initSubscription : Members
initSubscription =
    Members []


initProvider : Provider
initProvider =
    Provider initProfile initTopics initPortfolio initPortfolio [] initSubscription initSubscription


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
    , bio = undefined
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


type alias Link =
    { id : Int
    , profileId : Id
    , title : Title
    , url : Url
    , topics : List Topic
    , contentType : ContentType
    , isFeatured : Bool
    }


type alias LinkToCreate =
    { base : Link, currentTopic : Topic }


initLink : Link
initLink =
    { id = -1
    , profileId = Id undefined
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
    { id : Id
    , profileId : Id
    , platform : String
    , username : String
    , apiKey : String
    , links : List Link
    }


initSource : Source
initSource =
    Source (Id undefined) (Id undefined) "" "" "" []


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


toggleFilter : Provider -> ( Topic, Bool ) -> Provider
toggleFilter provider ( topic, include ) =
    let
        toggleTopic contentType existing =
            if include then
                existing
                    |> List.append (provider.portfolio |> getLinks contentType)
                    |> List.filter (\link -> (link.topics |> hasMatch topic))
            else
                existing |> List.filter (\link -> not (link.topics |> hasMatch topic))

        filtered =
            provider.filteredPortfolio

        refresh include contentType filteredTypeLinks =
            if include then
                (filteredTypeLinks |> toggleTopic contentType) ++ (filteredTypeLinks)
            else
                (filteredTypeLinks |> toggleTopic contentType)
    in
        { provider
            | filteredPortfolio =
                { answers = filtered.answers |> refresh include Answer
                , articles = filtered.articles |> refresh include Article
                , videos = filtered.videos |> refresh include Video
                , podcasts = filtered.podcasts |> refresh include Podcast
                }
        }


compareLinks : Link -> Link -> Order
compareLinks a b =
    if a.isFeatured then
        LT
    else if b.isFeatured then
        GT
    else
        EQ


getLinks : ContentType -> Portfolio -> List Link
getLinks contentType links =
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
            links.answers
                ++ links.articles
                ++ links.podcasts
                ++ links.videos


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
    topics |> List.map (\topic -> topic.name)


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

        Unknown ->
            "Unknown"

        All ->
            "Content"


allContentUrl : Linksfrom -> Id -> ContentType -> Url
allContentUrl linksFrom id contentType =
    case linksFrom of
        FromOther ->
            Url <| "/#/provider/" ++ idText id ++ "/all/" ++ (contentType |> contentTypeToText)

        FromPortal ->
            Url <| "/#/portal/" ++ idText id ++ "/all/" ++ (contentType |> contentTypeToText)


allTopicContentUrl : Linksfrom -> Id -> ContentType -> Topic -> Url
allTopicContentUrl linksFrom id contentType topic =
    case linksFrom of
        FromOther ->
            Url <| "/#/provider/" ++ idText id ++ "/" ++ topicText topic ++ "/all/" ++ (contentType |> contentTypeToText)

        FromPortal ->
            Url <| "/#/portal/" ++ idText id ++ "/" ++ topicText topic ++ "/all/" ++ (contentType |> contentTypeToText)
