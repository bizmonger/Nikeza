module Controls.AddLink exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)


-- MODEL


type alias Model =
    Link


type Msg
    = InputTitle String
    | InputUrl String
    | InputTopics (List String)
    | AddLink Model



-- UPDATE


update : Msg -> Model -> Model
update msg model =
    case msg of
        InputTitle v ->
            { model | title = Title v }

        InputUrl v ->
            { model | url = Url v }

        InputTopics v ->
            model

        AddLink v ->
            v



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ input [ type_ "text", placeholder "title", onInput InputTitle ] []
        , input [ type_ "text", placeholder "link", onInput InputUrl ] []
        , select []
            [ option [ value "undefined2" ] [ text "Select Type" ]
            , option [ value "Article2" ] [ text "Article" ]
            , option [ value "Video2" ] [ text "Video" ]
            , option [ value "Answer2" ] [ text "Answer" ]
            , option [ value "Podcast2" ] [ text "Podcast" ]
            ]
        , button [ onClick <| AddLink model ] [ text "Add" ]
        ]
