module Services.Adapter exposing (..)

import Domain.Core exposing (..)
import Http exposing (..)
import Task exposing (succeed, perform)


httpSuccess : (Result Http.Error a -> msg) -> a -> Cmd msg
httpSuccess msg a =
    a
        |> Result.Ok
        |> msg
        |> Task.succeed
        |> Task.perform identity


type alias Loginfunction msg =
    Credentials -> (Result Http.Error JsonProvider -> msg) -> Cmd msg


type alias Registerfunction msg =
    Form -> (Result Http.Error JsonProfile -> msg) -> Cmd msg


type alias Providerfunction msg =
    Id -> (Result Http.Error JsonProvider -> msg) -> Cmd msg


type alias FeaturedTopicsfunction msg =
    Id -> List String -> (Result Http.Error JsonProvider -> msg) -> Cmd msg


type alias Providersfunction msg =
    (Result Http.Error (List JsonProvider) -> msg) -> Cmd msg


type alias UpdateProfilefunction msg =
    Profile -> (Result Http.Error JsonProfile -> msg) -> Cmd msg


type alias UpdateProfileAndTopicsfunction msg =
    ProfileAndTopics -> (Result Http.Error JsonProvider -> msg) -> Cmd msg


type alias ThumbnailFunction msg =
    Platform -> String -> (Result Http.Error JsonThumbnail -> msg) -> Cmd msg


type alias SaveThumbnailfunction msg =
    UpdateThumbnailRequest -> (Result Http.Error String -> msg) -> Cmd msg


type alias AddSourcefunction msg =
    Source -> (Result Http.Error JsonSource -> msg) -> Cmd msg


type alias RemoveSourcefunction msg =
    Id -> (Result Http.Error String -> msg) -> Cmd msg


type alias Platformsfunction msg =
    (Result Http.Error (List String) -> msg) -> Cmd msg


type alias Bootstrapfunction msg =
    (Result Http.Error JsonBootstrap -> msg) -> Cmd msg


type alias Followersfunction msg =
    Id -> (Result Http.Error (List JsonProvider) -> msg) -> Cmd msg


type alias Subscriptionsfunction msg =
    Id -> (Result Http.Error (List JsonProvider) -> msg) -> Cmd msg


type alias Sourcesfunction msg =
    Id -> (Result Http.Error (List JsonSource) -> msg) -> Cmd msg


type alias AddLinkfunction msg =
    Link -> (Result Http.Error JsonLink -> msg) -> Cmd msg


type alias RemoveLinkfunction msg =
    Link -> (Result Http.Error JsonLink -> msg) -> Cmd msg


type alias Portfoliofunction msg =
    Id -> (Result Http.Error JsonPortfolio -> msg) -> Cmd msg


type alias TopicLinksfunction msg =
    Id -> Topic -> ContentType -> (Result Http.Error (List JsonLink) -> msg) -> Cmd msg


type alias Followfunction msg =
    SubscriptionRequest -> (Result Http.Error JsonProvider -> msg) -> Cmd msg


type alias Unsubscribefunction msg =
    SubscriptionRequest -> (Result Http.Error JsonProvider -> msg) -> Cmd msg



-- type alias RecentLinksfunction msg =
--     Id -> (Result Http.Error (List JsonLink) -> msg) -> Cmd msg


type alias RecentLinkProvidersfunction msg =
    Id -> (Result Http.Error (List JsonProvider) -> msg) -> Cmd msg


type alias FeatureLinkfunction msg =
    FeatureLink -> (Result Http.Error Int -> msg) -> Cmd msg


type alias SuggestedTopicsfunction msg =
    String -> (Result Http.Error (List String) -> msg) -> Cmd msg


type alias FeatureLink =
    { linkId : Int
    , isFeatured : Bool
    }


type alias JsonProfile =
    { id : String
    , firstName : String
    , lastName : String
    , email : String
    , imageUrl : String
    , bio : String
    , sources : List JsonSource
    }


type alias JsonTopic =
    { name : String
    , isFeatured : Bool
    }


type JsonProviderLinks
    = JsonProviderLinks JsonLinkFields


type alias JsonLinkFields =
    { links : List JsonLink
    }


type alias JsonSource =
    { id : Int
    , profileId : String
    , platform : String
    , accessId : String
    , links : List JsonLink
    }


type alias JsonPortfolio =
    { articles : List JsonLink
    , videos : List JsonLink
    , podcasts : List JsonLink
    , answers : List JsonLink
    }


type alias JsonThumbnail =
    { imageUrl : String
    , platform : String
    }


type alias JsonBootstrap =
    { providers : List JsonProvider
    , platforms : List String
    }


type alias JsonLink =
    { id : Int
    , profileId : String
    , title : String
    , url : String
    , contentType : String
    , topics : List Topic
    , isFeatured : Bool
    }


type JsonProvider
    = JsonProvider JsonProviderFields


type alias ProfileAndTopics =
    { profile : Profile
    , topics : List Topic
    }


type alias JsonProfileAndTopics =
    { profile : JsonProfile
    , topics : List JsonTopic
    }


type alias JsonProviderFields =
    { profile : JsonProfile
    , topics : List JsonTopic
    , portfolio : JsonPortfolio
    , recentLinks : List JsonLink
    , subscriptions : List String
    , followers : List String
    }


toProfile : JsonProfile -> Profile
toProfile jsonProfile =
    { id = Id jsonProfile.id
    , firstName = Name jsonProfile.firstName
    , lastName = Name jsonProfile.lastName
    , email = Email jsonProfile.email
    , imageUrl = Url jsonProfile.imageUrl
    , bio = jsonProfile.bio
    , sources = jsonProfile.sources |> List.map toSource
    }


toJsonProfile : Profile -> JsonProfile
toJsonProfile profile =
    { id = idText profile.id
    , firstName = nameText profile.firstName
    , lastName = nameText profile.lastName
    , email = emailText profile.email
    , imageUrl = urlText profile.imageUrl
    , bio = profile.bio
    , sources = profile.sources |> List.map toJsonSource
    }


toJsonPortfolio : Portfolio -> JsonPortfolio
toJsonPortfolio portfolio =
    { answers = portfolio.answers |> List.map (\l -> l |> toJsonLink)
    , articles = portfolio.articles |> List.map (\l -> l |> toJsonLink)
    , videos = portfolio.videos |> List.map (\l -> l |> toJsonLink)
    , podcasts = portfolio.podcasts |> List.map (\l -> l |> toJsonLink)
    }


toJsonProvider : Provider -> JsonProvider
toJsonProvider provider =
    JsonProvider
        { profile = provider.profile |> toJsonProfile
        , topics = provider.topics
        , portfolio = provider.portfolio |> toJsonPortfolio
        , recentLinks = provider.recentLinks |> List.map (\l -> l |> toJsonLink)
        , subscriptions = provider.subscriptions |> List.map (\s -> s |> idText)
        , followers = provider.followers |> List.map (\s -> s |> idText)
        }


toJsonSource : Source -> JsonSource
toJsonSource source =
    { id =
        case source.id |> idText |> String.toInt of
            Ok id ->
                id

            Err _ ->
                -1
    , profileId = idText source.profileId
    , platform = source.platform
    , accessId = source.accessId
    , links = source.links |> List.map toJsonLink
    }


toSource : JsonSource -> Source
toSource jsonSource =
    { id = jsonSource.id |> toString |> Id
    , profileId = jsonSource.profileId |> toString |> Id
    , platform = jsonSource.platform
    , accessId = jsonSource.accessId
    , links = jsonSource.links |> List.map toLink
    }


toJsonLinks : List Link -> List JsonLink
toJsonLinks links =
    links |> List.map toJsonLink


jsonProfileToProvider : JsonProfile -> Provider
jsonProfileToProvider jsonProfile =
    Provider (toProfile jsonProfile) initTopics initPortfolio initPortfolio [] initSubscription initSubscription


toMembers : List JsonProvider -> Members
toMembers jsonProviders =
    jsonProviders
        |> List.map toProvider
        |> Members


toLink : JsonLink -> Link
toLink link =
    { id = link.id
    , profileId = Id (link.profileId |> toString)
    , title = Title link.title
    , url = Url link.url
    , contentType = link.contentType |> toContentType
    , topics = link.topics
    , isFeatured = link.isFeatured
    }


toLinks : List JsonLink -> List Link
toLinks jsonLinks =
    jsonLinks |> List.map toLink


toJsonLink : Link -> JsonLink
toJsonLink link =
    { id = link.id
    , profileId = idText link.profileId
    , title = titleText link.title
    , url = urlText link.url
    , contentType = link.contentType |> contentTypeToText
    , topics = link.topics
    , isFeatured = link.isFeatured
    }


toPortfolio : JsonPortfolio -> Portfolio
toPortfolio jsonPortfolio =
    Portfolio
        []
        (jsonPortfolio.articles |> toLinks)
        (jsonPortfolio.videos |> toLinks)
        (jsonPortfolio.podcasts |> toLinks)
        (jsonPortfolio.answers |> toLinks)


toProvider : JsonProvider -> Provider
toProvider jsonProvider =
    let
        (JsonProvider field) =
            jsonProvider
    in
        { profile = field.profile |> toProfile
        , topics = field.topics
        , portfolio = field.portfolio |> toPortfolio
        , filteredPortfolio = field.portfolio |> toPortfolio
        , recentLinks = field.recentLinks |> toLinks
        , followers = field.followers |> List.map (\x -> Id x)
        , subscriptions = field.subscriptions |> List.map (\x -> Id x)
        }
