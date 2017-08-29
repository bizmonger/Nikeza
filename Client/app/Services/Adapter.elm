module Services.Adapter exposing (..)

import Domain.Core exposing (..)
import Http


type alias Loginfunction msg =
    Credentials -> (Result Http.Error JsonProvider -> msg) -> Cmd msg


type alias Registerfunction msg =
    Form -> (Result Http.Error JsonProfile -> msg) -> Cmd msg


type alias JsonProfile =
    { id : String
    , firstName : String
    , lastName : String
    , email : String
    }


type alias JsonTopic =
    { name : String
    , isFeatured : Bool
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


type alias JsonProvider =
    { profile : JsonProfile
    , topics : List JsonTopic
    , links : JsonLinks
    , recentLinks : JsonLinks
    , subscriptions : List JsonProfile
    , followers : List JsonProfile
    }


type alias JsonFollower =
    {}


type alias JsonSubscription =
    {}


jsonProfileToProfile : JsonProfile -> Profile
jsonProfileToProfile jsonProfile =
    { id = Id (jsonProfile.id |> toString)
    , firstName = Name jsonProfile.firstName
    , lastName = Name jsonProfile.lastName
    , email = Email jsonProfile.email
    , imageUrl = Url undefined
    , bio = undefined
    , sources = []
    }


jsonProfileToProvider : JsonProfile -> Provider
jsonProfileToProvider jsonProfile =
    Provider (jsonProfileToProfile jsonProfile) initTopics initLinks [] initSubscription initSubscription


toFollowers : List JsonFollower -> List Follower
toFollowers jsonFollowers =
    []


toSubscriptions : List JsonSubscription -> List Subscriber
toSubscriptions jsonSubscriptions =
    []


toLinks : JsonLinks -> Links
toLinks jsonLinks =
    initLinks


toTopics : List JsonTopic -> List Topic
toTopics jsonTopic =
    initProfile


toProfile : JsonProfile -> Profile
toProfile jsonProfile =
    initProfile


toProvider : JsonProvider -> Provider
toProvider jsonProvider =
    { profile = jsonProvider.profile |> toProfile
    , topics = jsonProvider.topics |> toTopics
    , links = jsonProvider.links |> toLinks
    , recentLinks = jsonProvider.recentLinks |> toLinks
    , subscriptions = jsonProvider.subscriptions |> toSubscriptions
    , followers = jsonProvider.followers |> toFollowers
    }
