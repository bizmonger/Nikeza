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
    , isInitialized : Bool
    }



-- main : Id -> Program Never Model Msg
-- main profileId =
--     Html.program
--         { init = ( init, runtime.sources profileId SourcesResponse )
--         , view = view
--         , update = update
--         , subscriptions = (\_ -> Sub.none)
--         }
-- init: Model -> (Model, Cmd Msg)
-- init model =
--     (model, runtime.sources model SourcesResponse)


type Msg
    = InputUsername String
    | InputPlatform String
    | Add Source
    | AddResponse (Result Http.Error JsonSource)
    | Remove Source
    | RemoveResponse (Result Http.Error JsonSource)
    | SourcesResponse (Result Http.Error (List JsonSource))



-- UPDATE


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    if not model.isInitialized then
        ( model, runtime.sources model.source.profileId SourcesResponse )
    else
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
                        | sources = toSource jsonSource :: model.sources
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

                SourcesResponse (Ok jsonSources) ->
                    ( { model | sources = jsonSources |> List.map toSource, isInitialized = True }, Cmd.none )

                SourcesResponse (Err _) ->
                    ( model, Cmd.none )


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
                    "user-id (Example: 492701)"

                "WordPress" ->
                    "xyz.wordpress.com"

                _ ->
                    "username"

        records =
            [ tr []
                [ td [] [ select [ class "selectPlatform", changeHandler, value model.source.platform ] <| instruction :: (model.platforms |> List.map platformOption) ]
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
            , td [] [ text <| toString model ]
            ]


sourceUI : Source -> Html Msg
sourceUI source =
    tr [ class "sources" ]
        [ td [] [ text source.platform ]
        , td [] [ i [ class "accessId" ] [ text source.username ] ]
        , td [] [ label [ class "linksCount" ] [ text <| (source.links |> List.length |> toString) ++ " links" ] ]
        , td [] [ button [ class "disconnectSource", onClick <| Remove source ] [ text "Disconnect" ] ]
        ]
