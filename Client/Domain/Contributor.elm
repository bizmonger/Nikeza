module Domain.Contributor exposing (..)

import Html exposing (..)
import Domain.Core exposing (..)
import Settings exposing (..)


main =
    Html.beginnerProgram
        { model = model
        , update = update
        , view = view
        }



-- MODEL


type alias Model =
    { profileId : Id
    , topics : List Topic
    , articles : List Post
    , videos : List Post
    , podcasts : List Post
    }


model : Model
model =
    { profileId = Id "undefined"
    , topics = []
    , articles = []
    , videos = []
    , podcasts = []
    }


type Msg
    = TopicsSelected
    | ArticlesSelected
    | VideosSelected
    | PodcastsSelected


update : Msg -> Model -> Model
update msg model =
    case msg of
        TopicsSelected ->
            { model | topics = [] }

        ArticlesSelected ->
            { model | articles = runtime.latestPosts model.profileId Articles }

        VideosSelected ->
            { model | videos = runtime.latestPosts model.profileId Videos }

        PodcastsSelected ->
            { model | podcasts = runtime.latestPosts model.profileId Podcasts }



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ p [] [ text "Welcome to the Contributors Page..." ]
        ]
