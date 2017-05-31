module Domain.Contributor exposing (..)

import Html exposing (..)
import Domain.Core exposing (..)
import Services.Server as Services exposing (tryLogin)
import Tests.TestAPI as TestAPI exposing (recentPodcasts, recentVideos, recentArticles)


-- MODEL


type alias Model =
    { profileId : Id
    , runtime : Dependencies
    , topics : List Topic
    , articles : List Article Post
    , videos : List Video Post
    , podcasts : List Podcast Post
    }


type Msg
    = Topics (List Topic)
    | Articles (List Article Post)
    | Videos (List Video Post)
    | Podcasts (List Podcast Post)


update : Msg -> Model -> Model
update msg model =
    case msg of
        Topics v ->
            { model | topics = v }

        Articles v ->
            { model | articles = v }

        Videos v ->
            { model | videos = v }

        Podcasts v ->
            { model | podcasts = v }



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ p [] []
        ]
