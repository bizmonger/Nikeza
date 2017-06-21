module Controls.AddSource exposing (..)

import Domain.Core exposing (..)
import Settings exposing (runtime)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import Json.Decode exposing (map)


-- MODEL


type alias Model =
    { source : Source, sources : List Source }


type Msg
    = InputUsername String
    | InputPlatform String
    | Add Source
    | Remove Source



-- UPDATE


update : Msg -> Model -> Model
update msg model =
    let
        source =
            model.source
    in
        case msg of
            InputUsername v ->
                { model | source = { source | username = v } }

            InputPlatform v ->
                { model | source = { source | platform = v } }

            Add source ->
                { model | sources = source :: model.sources }

            Remove v ->
                { model | sources = model.sources |> List.filter (\s -> s /= v) }


view : Model -> Html Msg
view model =
    let
        instruction =
            (option [ value "instructions" ] [ text "Select Platform" ])

        platformOption platform =
            option [ value <| getPlatform platform ] [ text <| getPlatform platform ]

        changeHandler =
            Html.Events.on "change" (Json.Decode.map InputPlatform Html.Events.targetValue)

        records =
            [ tr [] [ th [] [ h3 [] [ text "Data Sources" ] ] ]
            , tr []
                [ td [] [ select [ changeHandler, value model.source.platform ] <| instruction :: (runtime.platforms |> List.map platformOption) ]
                , td [] [ input [ type_ "text", placeholder "username", onInput InputUsername, value model.source.username ] [] ]
                ]
            , tr [] [ td [] [ button [ onClick <| Add model.source ] [ text "Add" ] ] ]
            ]

        tableRecords =
            List.append records (model.sources |> List.map sourceUI)
    in
        div [] [ table [] tableRecords ]


sourceUI : Source -> Html Msg
sourceUI source =
    tr []
        [ td [] [ text source.platform ]
        , td [] [ i [] [ text source.username ] ]
        , td [] [ button [ onClick <| Remove source ] [ text "Disconnect" ] ]
        ]
