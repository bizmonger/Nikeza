module Services.Adapter exposing (..)

import Domain.Core exposing (..)
import Http


type alias Loginfunction msg =
    Credentials -> (Result Http.Error JsonProvider -> msg) -> Cmd msg


type alias Registerfunction msg =
    Form -> (Result Http.Error JsonProfile -> msg) -> Cmd msg


type alias Providerfunction msg =
    Id -> (Result Http.Error JsonProvider -> msg) -> Cmd msg


type alias ProviderTopicfunction msg =
    Id -> Topic -> (Result Http.Error JsonProvider -> msg) -> Cmd msg


type alias Providersfunction msg =
    (Result Http.Error (List JsonProvider) -> msg) -> Cmd msg


type alias AddSourcefunction msg =
    Id -> Source -> (Result Http.Error (List Source) -> msg) -> Cmd msg


type alias RemoveSourcefunction msg =
    Id -> Source -> (Result Http.Error (List Source) -> msg) -> Cmd msg


type alias Followersfunction msg =
    Id -> (Result Http.Error Members -> msg) -> Cmd msg


type alias Subscriptionsfunction msg =
    Id -> (Result Http.Error Members -> msg) -> Cmd msg


type alias Sourcesfunction msg =
    Id -> (Result Http.Error (List Source) -> msg) -> Cmd msg


type alias AddLinkfunction msg =
    Id -> Link -> (Result Http.Error JsonPortfolio -> msg) -> Cmd msg


type alias RemoveLinkfunction msg =
    Id -> Link -> (Result Http.Error JsonPortfolio -> msg) -> Cmd msg


type alias Linksfunction msg =
    Id -> (Result Http.Error JsonPortfolio -> msg) -> Cmd msg


type alias TopicLinksfunction msg =
    Id -> Topic -> ContentType -> (Result Http.Error (List JsonLink) -> msg) -> Cmd msg


type alias JsonProfile =
    { id : String
    , firstName : String
    , lastName : String
    , email : String
    , imageUrl : String
    , bio : String
    , sources : List Source
    }


type alias JsonTopic =
    { name : String
    , isFeatured : Bool
    }


type alias JsonSource =
    { platform : String
    , username : String
    , linksFound : Int
    }


type alias JsonPortfolio =
    { articles : List JsonLink
    , videos : List JsonLink
    , podcasts : List JsonLink
    , answers : List JsonLink
    }


type alias JsonLink =
    { profile : JsonProfile
    , title : String
    , url : String
    , contentType : String
    , topics : List Topic
    , isFeatured : Bool
    }


type JsonProvider
    = JsonProvider JsonProviderFields


type alias JsonProviderFields =
    { profile : JsonProfile
    , topics : List JsonTopic
    , portfolio : JsonPortfolio
    , recentLinks : List JsonLink
    , subscriptions : List JsonProvider
    , followers : List JsonProvider
    }


toProfile : JsonProfile -> Profile
toProfile jsonProfile =
    { id = Id jsonProfile.id
    , firstName = Name jsonProfile.firstName
    , lastName = Name jsonProfile.lastName
    , email = Email jsonProfile.email
    , imageUrl = Url jsonProfile.imageUrl
    , bio = jsonProfile.bio
    , sources = jsonProfile.sources
    }


toJsonProfile : Profile -> JsonProfile
toJsonProfile profile =
    { id = idText profile.id
    , firstName = nameText profile.firstName
    , lastName = nameText profile.lastName
    , email = emailText profile.email
    , imageUrl = urlText profile.imageUrl
    , bio = profile.bio
    , sources = profile.sources
    }


jsonProfileToProvider : JsonProfile -> Provider
jsonProfileToProvider jsonProfile =
    Provider (toProfile jsonProfile) initTopics initPortfolio initPortfolio [] initSubscription initSubscription


toMembers : List JsonProvider -> Members
toMembers jsonProviders =
    Members (jsonProviders |> List.map (\p -> p |> toProvider))


toLinks : List JsonLink -> List Link
toLinks jsonLinks =
    jsonLinks
        |> List.map
            (\link ->
                { profile = link.profile |> toProfile
                , title = Title link.title
                , url = Url link.url
                , contentType = link.contentType |> toContentType
                , topics = link.topics
                , isFeatured = link.isFeatured
                }
            )


toJsonLink : Link -> JsonLink
toJsonLink link =
    { profile = link.profile |> toJsonProfile
    , title = titleText link.title
    , url = urlText link.url
    , contentType = link.contentType |> contentTypeToText
    , topics = link.topics
    , isFeatured = link.isFeatured
    }


toJsonLinks : List Link -> List JsonLink
toJsonLinks links =
    links |> List.map toJsonLink


toPortfolio : JsonPortfolio -> Portfolio
toPortfolio jsonPortfolio =
    Portfolio
        (jsonPortfolio.articles |> toLinks)
        (jsonPortfolio.videos |> toLinks)
        (jsonPortfolio.podcasts |> toLinks)
        (jsonPortfolio.answers |> toLinks)


toTopics : List JsonTopic -> List Topic
toTopics jsonTopics =
    jsonTopics |> List.map (\t -> { name = t.name, isFeatured = t.isFeatured })


toProvider : JsonProvider -> Provider
toProvider jsonProvider =
    let
        (JsonProvider field) =
            jsonProvider
    in
        { profile = field.profile |> toProfile
        , topics = field.topics |> toTopics
        , portfolio = field.portfolio |> toPortfolio
        , filteredPortfolio = field.portfolio |> toPortfolio
        , recentLinks = field.recentLinks |> toLinks
        , followers = field.followers |> toMembers
        , subscriptions = field.subscriptions |> toMembers
        }
