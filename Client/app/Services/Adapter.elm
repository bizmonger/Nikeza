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


type alias JsonLinks =
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
    , links : JsonLinks
    , recentLinks : List JsonLink
    , subscriptions : List JsonProvider
    , followers : List JsonProvider
    }


type alias JsonSubscriber =
    {}


jsonProfileToProfile : JsonProfile -> Profile
jsonProfileToProfile jsonProfile =
    { id = Id (jsonProfile.id |> toString)
    , firstName = Name jsonProfile.firstName
    , lastName = Name jsonProfile.lastName
    , email = Email jsonProfile.email
    , imageUrl = Url jsonProfile.imageUrl
    , bio = jsonProfile.bio
    , sources = []
    }


jsonProfileToProvider : JsonProfile -> Provider
jsonProfileToProvider jsonProfile =
    Provider (jsonProfileToProfile jsonProfile) initTopics initLinks [] initSubscription initSubscription


toFollowers : List JsonSubscriber -> List Profile
toFollowers jsonFollowers =
    [ initProfile ]


toSubscriptions : List JsonSubscriber -> List Profile
toSubscriptions jsonSubscriptions =
    [ initProfile ]


toLink : List JsonLink -> List Link
toLink jsonLinks =
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


jsonLinksToLinks : JsonLinks -> Links
jsonLinksToLinks jsonLinks =
    Links
        (jsonLinks.articles |> toLink)
        (jsonLinks.videos |> toLink)
        (jsonLinks.podcasts |> toLink)
        (jsonLinks.answers |> toLink)



-- { articles : List JsonLink
-- , videos : List JsonLink
-- , podcasts : List JsonLink
-- , answers : List JsonLink
-- }


toTopics : List JsonTopic -> List Topic
toTopics jsonTopic =
    initTopics


toProfile : JsonProfile -> Profile
toProfile jsonProfile =
    let
        ( id, email ) =
            ( Id jsonProfile.id, Email jsonProfile.email )

        ( firstName, lastName ) =
            ( Name jsonProfile.firstName, Name jsonProfile.lastName )

        ( imageUrl, bio, sources ) =
            ( Url jsonProfile.imageUrl, jsonProfile.bio, jsonProfile.sources )
    in
        Profile id firstName lastName email imageUrl bio sources


toProvider : JsonProvider -> Provider
toProvider jsonProvider =
    let
        (JsonProvider field) =
            jsonProvider
    in
        { profile = field.profile |> toProfile
        , topics = field.topics |> toTopics
        , links = field.links |> jsonLinksToLinks
        , recentLinks = field.recentLinks |> toLink
        , subscriptions = (\id -> Subscribers (field.subscriptions |> List.map (\jp -> jp |> toProvider)))
        , followers = (\id -> Subscribers (field.followers |> List.map (\jp -> jp |> toProvider)))
        }
