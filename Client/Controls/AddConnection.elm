module Controls.AddConnection exposing (..)

import Domain.Core exposing (..)
import Settings exposing (runtime)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import Json.Decode exposing (map)


-- MODEL


type alias Model =
    Connection


init : Connection
init =
    Connection "" ""


type Msg
    = InputUsername String
    | InputPlatform String
    | Submit Connection



-- UPDATE


update : Msg -> Model -> Model
update msg model =
    case msg of
        InputUsername v ->
            { model | username = v }

        InputPlatform v ->
            { model | platform = v }

        Submit connection ->
            connection


view : Model -> Html Msg
view model =
    let
        instruction =
            (option [ value "instructions" ] [ text "Select Platform" ])

        platformOption platform =
            option [ value <| getPlatform platform ] [ text <| getPlatform platform ]
    in
        div []
            [ select [ Html.Events.on "change" (Json.Decode.map InputPlatform Html.Events.targetValue), value model.platform ] <| instruction :: (runtime.platforms |> List.map platformOption)
            , input [ type_ "text", placeholder "username", onInput InputUsername, value model.username ] []
            , button [ onClick <| Submit model ] [ text "Add" ]
            ]
