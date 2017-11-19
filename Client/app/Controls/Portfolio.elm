module Controls.Portfolio exposing (..)

import Settings exposing (..)
import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onCheck, onInput)
import Http


-- UPDATE


type Msg
    = InputTopic String
    | TopicSuggestionResponse (Result Http.Error (List String))
    | TopicSelected Topic


update : Msg -> PortfolioSearch -> ( PortfolioSearch, Cmd Msg )
update msg model =
    case msg of
        InputTopic "" ->
            ( model, Cmd.none )

        InputTopic v ->
            ( model, runtime.suggestedTopics v TopicSuggestionResponse )

        TopicSuggestionResponse (Ok tags) ->
            let
                suggestions =
                    tags |> List.map (\t -> Topic t False)
            in
                ( { model | topicSuggestions = suggestions }, Cmd.none )

        TopicSuggestionResponse (Err reason) ->
            ( model, Cmd.none )

        TopicSelected topic ->
            let
                provider =
                    model.provider

                filtered =
                    provider.filteredPortfolio

                updatedFilter =
                    { filtered
                        | articles = filtered.articles |> List.filter (\l -> l.topics |> hasMatch topic)
                        , videos = filtered.videos |> List.filter (\l -> l.topics |> hasMatch topic)
                        , answers = filtered.answers |> List.filter (\l -> l.topics |> hasMatch topic)
                        , podcasts = filtered.podcasts |> List.filter (\l -> l.topics |> hasMatch topic)
                    }
            in
                ( { model | provider = { provider | filteredPortfolio = updatedFilter } }, Cmd.none )


view : Linksfrom -> PortfolioSearch -> Html Msg
view linksFrom model =
    let
        provider =
            model.provider

        profileId =
            provider.profile.id

        filtered =
            provider.filteredPortfolio

        ( answerCount, articleCount, podcastCount, videoCount ) =
            ( provider.portfolio |> getLinks Answer |> List.length
            , provider.portfolio |> getLinks Article |> List.length
            , provider.portfolio |> getLinks Podcast |> List.length
            , provider.portfolio |> getLinks Video |> List.length
            )

        toButton topic =
            div []
                [ button [ class "topicsButton", onClick <| TopicSelected topic ] [ text <| topicText topic ]
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
    in
        div []
            [ table []
                [ tr []
                    [ table []
                        [ tr []
                            [ td []
                                [ table []
                                    [ tr [] [ td [] [ input [ type_ "text", placeholder "search topic", onInput InputTopic ] [] ] ]
                                    , tr [] [ td [] [ suggestionsUI (model.topicSuggestions |> List.map (\t -> topicText t)) ] ]
                                    ]
                                ]
                            , table [ class "contentTable" ]
                                [ tr [ class "contentTypeHeader" ]
                                    [ td [] [ b [] [ text "Answers" ] ]
                                    , td [] [ b [] [ text "Articles" ] ]
                                    ]
                                , tr []
                                    [ td [ class "portfolioContent" ] [ div [ class "contentType" ] <| requestAllContent linksFrom profileId Answer answerCount filtered.answers ]
                                    , td [ class "portfolioContent" ] [ div [ class "contentType" ] <| requestAllContent linksFrom profileId Article articleCount filtered.articles ]
                                    ]
                                , tr [ class "contentTypeHeader" ]
                                    [ td [] [ b [] [ text "Podcasts" ] ]
                                    , td [] [ b [] [ text "Videos" ] ]
                                    ]
                                , tr []
                                    [ td [ class "portfolioContent" ] [ div [ class "contentType" ] <| requestAllContent linksFrom profileId Podcast podcastCount filtered.podcasts ]
                                    , td [ class "portfolioContent" ] [ div [ class "contentType" ] <| requestAllContent linksFrom profileId Video videoCount filtered.videos ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]


requestAllContent : Linksfrom -> Id -> ContentType -> Int -> List Link -> List (Html Msg)
requestAllContent linksFrom profileId contentType count links =
    List.append (linksUI links)
        [ p [ class "AllPortfolioLinks" ]
            [ a [ class "allLinks", href <| urlText <| allContentUrl linksFrom profileId contentType ]
                [ text <| ("  view all " ++ toString count ++ " links  "), br [] [] ]
            ]
        ]


formatTitle : Link -> String
formatTitle link =
    let
        ( maxLength, titleLength ) =
            ( 50, String.length <| titleText link.title )

        title =
            if titleLength > maxLength then
                let
                    partialTitle =
                        link.title
                            |> titleText
                            |> String.dropRight (titleLength - maxLength - 3)
                in
                    partialTitle ++ "..."
            else
                titleText link.title
    in
        title


decorateIfFeatured : Link -> Html Msg
decorateIfFeatured link =
    if not link.isFeatured then
        p [ class "portfolioLink" ] [ a [ href <| urlText link.url, target "_blank" ] [ text <| formatTitle link, br [] [] ] ]
    else
        p [] [ a [ href <| urlText link.url, target "_blank" ] [ text <| formatTitle link, br [] [] ] ]


linksUI : List Link -> List (Html Msg)
linksUI links =
    links
        |> List.sortWith compareLinks
        |> List.take 5
        |> List.map decorateIfFeatured
