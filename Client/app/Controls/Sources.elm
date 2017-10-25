module Controls.Sources exposing (..)

import Settings exposing (runtime)
import Domain.Core exposing (..)
import Services.Adapter exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import Http
import Json.Decode exposing (map)


-- MODEL


type alias Model =
    { source : Source
    , sources : List Source
    }


type Msg
    = InputUsername String
    | InputPlatform String
    | Add Source
    | AddResponse (Result Http.Error JsonSource)
    | Remove Source
    | RemoveResponse (Result Http.Error JsonSource)



-- UPDATE


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    let
        source =
            model.source
    in
        case msg of
            InputUsername v ->
                ( { model | source = { source | username = v } }, Cmd.none )

            InputPlatform v ->
                ( { model | source = { source | platform = v } }, Cmd.none )

            Add source ->
                ( model, runtime.addSource source AddResponse )

            Remove source ->
                ( model, runtime.removeSource source.id RemoveResponse )

            AddResponse (Ok jsonSource) ->
                ( { model
                    | sources = (jsonSource |> toSource) :: model.sources
                    , source = initSource
                  }
                , Cmd.none
                )

            AddResponse (Err error) ->
                Debug.crash (toString error) ( model, Cmd.none )

            RemoveResponse (Ok jsonSource) ->
                ( { model | sources = model.sources |> List.filter (\s -> s /= (jsonSource |> toSource)) }, Cmd.none )

            RemoveResponse (Err error) ->
                Debug.crash (toString error) ( model, Cmd.none )


view : Model -> List Platform -> Html Msg
view model platforms =
    let
        instruction =
            (option [ value "instructions" ] [ text "Select Platform" ])

        platformOption platform =
            option [ value <| platformText platform ] [ text <| platformText platform ]

        changeHandler =
            Html.Events.on "change" (Json.Decode.map InputPlatform Html.Events.targetValue)

        placeholderText =
            if model.source.platform == "YouTube" then
                "channel-id: (In Settings)"
            else
                "username"

        records =
            [ tr []
                [ td [] [ select [ class "selectPlatform", changeHandler, value model.source.platform ] <| instruction :: (platforms |> List.map platformOption) ]
                , td [] [ input [ class "inputUsername", type_ "text", placeholder placeholderText, onInput InputUsername, value model.source.username ] [] ]
                , td [] [ button [ class "addSource", onClick <| Add model.source ] [ text "Add" ] ]
                ]
            ]

        tableRecords =
            List.append records (model.sources |> List.map sourceUI)
    in
        div [ class "mainContent" ]
            [ h3 [] [ text "Sources" ]
            , table [] tableRecords
            ]


sourceUI : Source -> Html Msg
sourceUI source =
    tr [ class "sources" ]
        [ td [] [ text source.platform ]
        , td [] [ i [] [ text source.username ] ]
        , td [] [ text <| "(" ++ (source.links |> List.length |> toString) ++ ") links" ]
        , td [] [ button [ onClick <| Remove source ] [ text "Disconnect" ] ]
        ]
