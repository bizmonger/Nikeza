module Controls.NewLinks exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import Json.Decode exposing (map)
import Style exposing (listStyleType)


-- MODEL


init =
    { current = initLink, canAdd = False, added = [] }


type alias Model =
    AddedLinks


type Msg
    = InputTitle String
    | InputUrl String
    | InputTopic String
    | InputContentType String
    | AddLink Model



-- UPDATE


update : Msg -> Model -> Model
update msg model =
    let
        link =
            model.current
    in
        case msg of
            InputTitle v ->
                { model | current = { link | title = Title v } }

            InputUrl v ->
                { model | current = { link | url = Url v } }

            InputTopic v ->
                { model | current = { link | currentTopic = Topic v } }

            InputContentType v ->
                { model | current = { link | contentType = toContentType v } }

            AddLink v ->
                v



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ input [ type_ "text", placeholder "title", onInput InputTitle, value <| getTitle model.current.title ] []
        , input [ type_ "text", placeholder "link", onInput InputUrl, value <| getUrl model.current.url ] []
        , br [] []
        , input [ type_ "text", placeholder "topic", onInput InputTopic, value (getTopic model.current.currentTopic) ] []
        , select [ Html.Events.on "change" (Json.Decode.map InputContentType Html.Events.targetValue) ]
            [ option [ value "Undefined" ] [ text "Select Type" ]
            , option [ value "Article" ] [ text "Article" ]
            , option [ value "Video" ] [ text "Video" ]
            , option [ value "Answer" ] [ text "Answer" ]
            , option [ value "Podcast" ] [ text "Podcast" ]
            ]
        , br [] []
        , button [ onClick <| AddLink model ] [ text "Add" ]
        ]
