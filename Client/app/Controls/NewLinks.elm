module Controls.NewLinks exposing (..)

import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import Json.Decode exposing (map)
import Http
import Settings exposing (..)
import Domain.Core exposing (..)
import Services.Adapter exposing (..)


type Msg
    = InputTitle String
    | InputUrl String
    | InputTopic String
    | RemoveTopic Topic
    | InputContentType String
    | AddLink NewLinks
    | AddTopic Topic
    | TopicSuggestionResponse (Result Http.Error (List String))
    | Response (Result Http.Error JsonLink)


update : Msg -> NewLinks -> ( NewLinks, Cmd Msg )
update msg model =
    let
        linkToCreate =
            model.current

        linkToCreateBase =
            model.current.base
    in
        case msg of
            InputTitle v ->
                ( { model | current = { linkToCreate | base = { linkToCreateBase | title = Title v } } }, Cmd.none )

            InputUrl v ->
                ( { model | current = { linkToCreate | base = { linkToCreateBase | url = Url v } } }, Cmd.none )

            InputTopic "" ->
                ( { model | current = { linkToCreate | currentTopic = Topic "" False } }, Cmd.none )

            InputTopic v ->
                ( { model | current = { linkToCreate | currentTopic = Topic v False } }, runtime.suggestedTopics v TopicSuggestionResponse )

            RemoveTopic v ->
                let
                    link =
                        { linkToCreateBase | topics = linkToCreateBase.topics |> List.filter (\t -> t /= v) }
                in
                    ( { model | current = { linkToCreate | base = link } }, Cmd.none )

            AddTopic v ->
                ( { model
                    | current =
                        { linkToCreate
                            | currentTopic = Topic "" False
                            , topicSuggestions = []
                            , base =
                                { linkToCreateBase | topics = v :: linkToCreateBase.topics }
                        }
                  }
                , Cmd.none
                )

            InputContentType v ->
                ( { model | current = { linkToCreate | base = { linkToCreateBase | contentType = toContentType v } } }, Cmd.none )

            AddLink v ->
                let
                    ( link, current ) =
                        ( v.current.base, v.current )

                    updatedCurrent =
                        { current | base = { link | profileId = model.profileId }, topicSuggestions = [] }

                    preparedLink =
                        updatedCurrent.base
                in
                    ( { model | current = updatedCurrent }, runtime.addLink preparedLink Response )

            TopicSuggestionResponse (Ok topics) ->
                let
                    current =
                        model.current

                    suggestions =
                        topics |> List.map (\t -> Topic t False)
                in
                    ( { model | current = { current | topicSuggestions = suggestions } }, Cmd.none )

            TopicSuggestionResponse (Err reason) ->
                Debug.crash (toString reason) ( model, Cmd.none )

            Response (Ok jsonLink) ->
                ( { model
                    | added = (jsonLink |> toLink) :: model.added
                    , current = initLinkToCreate
                  }
                , Cmd.none
                )

            Response (Err error) ->
                Debug.crash ("Error: " ++ toString error) ( model, Cmd.none )



-- VIEW


view : NewLinks -> Html Msg
view model =
    let
        toButton topic =
            div []
                [ button [ class "topicsButton", onClick <| AddTopic topic ] [ text <| topicText topic ]
                , br [] []
                ]

        suggestionsUI textItems =
            let
                buttonsContainer =
                    textItems
                        |> List.map (\textItem -> Topic textItem False)
                        |> List.map (\t -> t |> toButton)
            in
                div [] buttonsContainer

        selectedTopicsUI =
            current.base.topics
                |> List.map
                    (\t ->
                        div []
                            [ label [ class "topicAdded" ] [ text <| topicText t ]
                            , button [ class "removeTopic", onClick <| RemoveTopic t ] [ text "X" ]
                            , br [] []
                            , br [] []
                            ]
                    )

        ( current, base ) =
            ( model.current, model.current.base )

        listbox =
            select [ Html.Events.on "change" (Json.Decode.map InputContentType Html.Events.targetValue) ]
                [ option [ value "instructions" ] [ text "Content Type" ]
                , option [ value "Article" ] [ text "Article" ]
                , option [ value "Video" ] [ text "Video" ]
                , option [ value "Answer" ] [ text "Answer" ]
                , option [ value "Podcast" ] [ text "Podcast" ]
                ]
    in
        div [ class "mainContent" ]
            [ h3 [ class "portalTopicHeader" ] [ text "Link" ]
            , table [ class "linkTable" ]
                [ tr []
                    [ td []
                        [ table []
                            [ tr []
                                [ td [] [ input [ class "addLinkText", type_ "text", placeholder "title", onInput InputTitle, value <| titleText base.title ] [] ]
                                ]
                            , tr [] [ td [] [ input [ class "addLinkText", type_ "text", placeholder "link url", onInput InputUrl, value <| urlText base.url ] [] ] ]
                            , tr []
                                [ td []
                                    [ table []
                                        [ tr []
                                            [ td [] [ input [ class "addTopic", type_ "text", placeholder "topic", onInput InputTopic, value (topicText current.currentTopic) ] [] ]
                                            , td [] [ listbox ]
                                            ]
                                        ]
                                    ]
                                ]
                            , tr [] [ td [] [ suggestionsUI (current.topicSuggestions |> List.map (\t -> topicText t)) ] ]
                            , tr [] [ td [] [ div [] selectedTopicsUI ] ]
                            ]
                        ]
                    , td [] [ button [ class "addLink", onClick <| AddLink model ] [ text "Add Link" ] ]
                    ]
                ]
            ]
