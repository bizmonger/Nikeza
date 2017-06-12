module Controls.AddConnection exposing (..)

import Domain.Core exposing (..)
import Settings exposing (runtime)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)


-- MODEL


type alias Model =
    { platform : String, username : String }


init : Model
init =
    Model "" ""


type Msg
    = InputUsername String
    | InputPlatform String
    | Submit Connection



-- UPDATE


update : Msg -> Model -> Model
update msg model =
    case msg of
        InputUsername username ->
            { model | username = username }

        InputPlatform platform ->
            { model | platform = platform }

        Submit connection ->
            model


view : Model -> Html Msg
view model =
    let
        instruction =
            (option [ value "instructions" ] [ text "Select Platform" ])

        platformOption platform =
            option [ value <| getPlatform platform ] [ text <| getPlatform platform ]
    in
        div []
            [ select [ onInput InputPlatform ] <| instruction :: (runtime.platforms |> List.map platformOption)
            , input [ type_ "text", placeholder "username", onInput InputUsername ] []
            , button [ onClick <| Submit model ] [ text "Add" ]
            ]
