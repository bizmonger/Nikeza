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
    { profileId : Id
    , platforms : List Platform
    , source : Source
    , sources : List Source
    }


type Msg
    = InputAccessId String
    | InputPlatform String
    | Add Source
    | AddResponse (Result Http.Error JsonSource)
    | Remove Source
    | RemoveResponse (Result Http.Error String)



-- UPDATE


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    let
        source =
            model.source
    in
        case msg of
            InputAccessId v ->
                ( { model | source = { source | accessId = v } }, Cmd.none )

            InputPlatform v ->
                ( { model | source = { source | platform = v } }, Cmd.none )

            Add source ->
                ( model, runtime.addSource source AddResponse )

            Remove source ->
                ( model, runtime.removeSource source.id RemoveResponse )

            AddResponse (Ok jsonSource) ->
                let
                    source =
                        toSource jsonSource
                in
                    ( { model
                        | sources = source :: model.sources
                        , source = initSource
                      }
                    , Cmd.none
                    )

            AddResponse (Err error) ->
                Debug.crash (toString error) ( model, Cmd.none )

            RemoveResponse (Ok sourceId) ->
                ( { model | sources = model.sources |> List.filter (\s -> (idText s.id) /= sourceId) }, Cmd.none )

            RemoveResponse (Err error) ->
                Debug.crash (toString error) ( model, Cmd.none )


view : Model -> Html Msg
view model =
    let
        instruction =
            (option [ value "instructions" ] [ text "Select Platform" ])

        platformOption platform =
            option [ value <| platformText platform ] [ text <| platformText platform ]

        changeHandler =
            Html.Events.on "change" (Json.Decode.map InputPlatform Html.Events.targetValue)

        placeholderText =
            case model.source.platform of
                "YouTube" ->
                    "channel-id: (Settings menu)"

                "StackOverflow" ->
                    "/users/ (i.e. 492701)"

                "WordPress" ->
                    "xyz.wordpress.com"

                "RSS Feed" ->
                    "i.e. http://xyz.com/feed/123"

                _ ->
                    "username"

        records =
            [ tr []
                [ td [] [ select [ class "selectPlatform", changeHandler, value model.source.platform ] <| instruction :: (model.platforms |> List.map platformOption) ]
                , td [] [ input [ class "inputUsername", type_ "text", placeholder placeholderText, onInput InputAccessId, value model.source.accessId ] [] ]
                , td [] [ button [ class "addSource", onClick <| Add model.source ] [ text "Add" ] ]
                ]
            ]

        tableRecords =
            List.append records (model.sources |> List.map sourceUI)
    in
        div [ class "mainContent" ]
            [ h3 [ class "portalTopicHeader" ] [ text "Sources" ]
            , table [] tableRecords
            ]


sourceUI : Source -> Html Msg
sourceUI source =
    let
        accessId =
            if String.length source.accessId > 28 then
                String.left 28 source.accessId ++ "..."
            else
                source.accessId
    in
        tr [ class "sources" ]
            [ td [] [ text source.platform ]
            , td [] [ i [ class "accessId" ] [ text accessId ] ]

            -- , td [] [ label [ class "linksCount" ] [ text <| (source.links |> List.length |> toString) ++ " links" ] ]
            , td [] [ button [ class "disconnectSource", onClick <| Remove source ] [ text "X" ] ]
            ]
