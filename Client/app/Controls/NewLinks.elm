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
    | KeyDown Int
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

        onAddTopic topic =
            ( { model
                | current =
                    { linkToCreate
                        | currentTopic = Topic "" False
                        , topicSuggestions = []
                        , base =
                            { linkToCreateBase | topics = topic :: linkToCreateBase.topics }
                    }
              }
            , Cmd.none
            )
    in
        case msg of
            InputTitle v ->
                ( { model | current = { linkToCreate | base = { linkToCreateBase | title = Title v } } }, Cmd.none )

            InputUrl v ->
                let
                    hasText domainName =
                        if model.current.base.url |> urlText |> String.toLower |> String.contains domainName then
                            True
                        else
                            False

                    link =
                        model.current.base

                    contentType =
                        if link.contentType == Unknown && v == "" then
                            Unknown
                        else if hasText "youtube.com" then
                            Video
                        else if hasText "vimeo.com" then
                            Video
                        else if hasText "wordpress.com" then
                            Article
                        else if hasText "medium.com" then
                            Article
                        else if hasText "stackoverflow.com" then
                            Answer
                        else if
                            hasText "itunes"
                                || hasText "soundcloud"
                                || hasText "dotnetrocks"
                                || hasText "developeronfire"
                                || hasText "testtalks"
                                || hasText "legacycoderocks"
                                || hasText "hanselminutes"
                                || hasText "se-radio"
                                || hasText "elixirfountain"
                                || hasText "rubyonrails"
                                || hasText "3devsandamaybe"
                                || hasText "lambdacast"
                                || hasText "functionalgeekery"
                                || hasText "herdingcode"
                                || hasText ".fm"
                                || hasText "/podcasts"
                                || hasText "runasradio"
                                || hasText "greaterthancode"
                        then
                            Podcast
                        else
                            Unknown
                in
                    ( { model | current = { linkToCreate | base = { linkToCreateBase | url = Url v, contentType = contentType } } }, Cmd.none )

            InputTopic "" ->
                ( { model | current = { linkToCreate | currentTopic = Topic "" False } }, Cmd.none )

            InputTopic v ->
                ( { model | current = { linkToCreate | currentTopic = Topic v False } }, runtime.suggestedTopics v TopicSuggestionResponse )

            KeyDown key ->
                if key == 13 then
                    case model.current.topicSuggestions of
                        topic :: _ ->
                            onAddTopic topic

                        _ ->
                            ( model, Cmd.none )
                else
                    ( model, Cmd.none )

            RemoveTopic v ->
                let
                    link =
                        { linkToCreateBase | topics = linkToCreateBase.topics |> List.filter (\t -> t /= v) }
                in
                    ( { model | current = { linkToCreate | base = link } }, Cmd.none )

            AddTopic v ->
                onAddTopic v

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

        ( isArticle, isVideo, isAnswer, isPodcast ) =
            case base.contentType of
                Video ->
                    ( False, True, False, False )

                Article ->
                    ( True, False, False, False )

                Answer ->
                    ( False, False, True, False )

                Podcast ->
                    ( False, False, True, True )

                Unknown ->
                    ( False, False, False, False )

                All ->
                    ( False, False, False, False )

                Featured ->
                    ( False, False, False, False )

        listview =
            select [ Html.Events.on "change" (Json.Decode.map InputContentType Html.Events.targetValue) ]
                [ option [ value "instructions" ] [ text "Content Type" ]
                , option [ value "Article", selected isArticle ] [ text "Article" ]
                , option [ value "Video", selected isVideo ] [ text "Video" ]
                , option [ value "Answer", selected isAnswer ] [ text "Answer" ]
                , option [ value "Podcast", selected isPodcast ] [ text "Podcast" ]
                ]

        titleCompleted =
            not (String.isEmpty (titleText base.title))

        urlCompleted =
            not (String.isEmpty (urlText base.url))

        hasContentType =
            model.current.base.contentType /= Unknown

        canAdd =
            titleCompleted && urlCompleted && hasContentType
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
                                            [ td [] [ input [ class "addTopic", type_ "text", placeholder "topic", onKeyDown KeyDown, onInput InputTopic, value (topicText current.currentTopic) ] [] ]
                                            , td [] [ listview ]
                                            ]
                                        ]
                                    ]
                                ]
                            , tr [] [ td [] [ suggestionsUI (current.topicSuggestions |> List.map (\t -> topicText t)) ] ]
                            , tr [] [ td [] [ div [] selectedTopicsUI ] ]
                            ]
                        ]
                    , td [] [ button [ class "addLink", onClick <| AddLink model, disabled (not canAdd) ] [ text "Add Link" ] ]
                    ]
                ]
            ]
