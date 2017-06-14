module Controls.NewLinks exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)


-- MODEL


init =
    { current = initLink, canAdd = False, added = [] }


type alias Model =
    AddedLinks


type Msg
    = InputTitle String
    | InputUrl String
    | InputTopics (List String)
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

            InputTopics v ->
                model

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
        , select []
            [ option [ value "undefined2" ] [ text "Select Type" ]
            , option [ value "Article2" ] [ text "Article" ]
            , option [ value "Video2" ] [ text "Video" ]
            , option [ value "Answer2" ] [ text "Answer" ]
            , option [ value "Podcast2" ] [ text "Podcast" ]
            ]
        , button [ onClick <| AddLink model ] [ text "Add" ]
        ]
