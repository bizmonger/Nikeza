module Domain.Core exposing (..)

import Controls.Login as Login exposing (Model)


type alias Profile =
    { id : Id
    , name : Submitter
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


type Submitter
    = Submitter String


getName : Submitter -> String
getName submitter =
    let
        (Submitter value) =
            submitter
    in
        value


type Title
    = Title String


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


gettopic : Topic -> String
gettopic topic =
    let
        (Topic value) =
            topic
    in
        value


type Video
    = Video Post


type Article
    = Article Post


type Podcast
    = Podcast Post


type alias Post =
    { submitter : Profile, title : Title, url : Url }


type alias Loginfunction =
    Login.Model -> Login.Model


tryLogin : Loginfunction -> String -> String -> Login.Model
tryLogin loginf username password =
    loginf <| Login.Model username password False


type alias TopicUrlFunction =
    Id -> Topic -> Url


topicUrl : TopicUrlFunction -> Id -> Topic -> Url
topicUrl f id topic =
    f id topic
