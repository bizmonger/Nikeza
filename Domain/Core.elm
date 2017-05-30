module Domain.Core exposing (..)

import Controls.Login as Login exposing (Model)


type Submitter
    = Submitter String


type Title
    = Title String


type Url
    = Url String


type Video
    = Video Post


type Article
    = Article Post


type Podcast
    = Podcast Post


type alias Post =
    { submitter : Submitter, title : Title, url : Url }


type alias Loginfunction =
    Login.Model -> Login.Model


tryLogin : Loginfunction -> String -> String -> Login.Model
tryLogin loginf username password =
    loginf <| Login.Model username password False
