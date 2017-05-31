module Domain.Core exposing (..)

import Controls.Login as Login exposing (Model)


type alias Profile =
    { id : Id
    , name : Submitter
    , imageUrl : Url
    , bio : String
    , tags : List Tag
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


type Tag
    = Tag String


getTag : Tag -> String
getTag tag =
    let
        (Tag value) =
            tag
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


type alias TagUrlFunction =
    Id -> Tag -> Url


tagUrl : TagUrlFunction -> Id -> Tag -> Url
tagUrl f id tag =
    f id tag
