module Controls.Portfolio exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onCheck, onInput)


type Msg
    = Toggle ( Topic, Bool )


update : Msg -> Provider -> Provider
update msg provider =
    case msg of
        Toggle ( topic, include ) ->
            toggleFilter provider ( topic, include )


view : Linksfrom -> Provider -> Html Msg
view linksFrom provider =
    let
        profileId =
            provider.profile.id

        toCheckBoxState include topic =
            div []
                [ input [ type_ "checkbox", checked include, onCheck (\isChecked -> Toggle ( topic, isChecked )) ] []
                , label [] [ text <| topicText topic ]
                ]

        filtered =
            provider.filteredPortfolio

        featuredArticles =
            if (filtered.articles |> List.filter (.isFeatured)) == [] then
                filtered.articles |> List.take 5 |> List.map (\l -> { l | isFeatured = True })
            else
                filtered.articles |> List.filter .isFeatured

        featuredVideos =
            if (filtered.videos |> List.filter (.isFeatured)) == [] then
                filtered.videos |> List.take 5 |> List.map (\l -> { l | isFeatured = True })
            else
                filtered.videos |> List.filter .isFeatured

        featuredAnswers =
            if (filtered.answers |> List.filter (.isFeatured)) == [] then
                filtered.answers |> List.take 5 |> List.map (\l -> { l | isFeatured = True })
            else
                filtered.answers |> List.filter .isFeatured

        featuredPodcasts =
            if (filtered.podcasts |> List.filter (.isFeatured)) == [] then
                filtered.podcasts |> List.take 5 |> List.map (\l -> { l | isFeatured = True })
            else
                filtered.podcasts |> List.filter .isFeatured

        ( answerCount, articleCount, podcastCount, videoCount ) =
            ( provider.portfolio |> getLinks Answer |> List.length
            , provider.portfolio |> getLinks Article |> List.length
            , provider.portfolio |> getLinks Podcast |> List.length
            , provider.portfolio |> getLinks Video |> List.length
            )
    in
        div []
            [ table []
                [ tr []
                    [ table []
                        [ tr []
                            [ td [] [ div [] <| (provider.portfolio |> getLinks Featured |> topicsFromLinks |> List.map (\t -> t |> toCheckBoxState True)) ]
                            , table [ class "contentTable" ]
                                [ tr [ class "contentTypeHeader" ]
                                    [ td [] [ b [] [ text "Answers" ] ]
                                    , td [] [ b [] [ text "Articles" ] ]
                                    ]
                                , tr []
                                    [ td [ class "portfolioContent" ] [ div [ class "contentType" ] <| requestAllContent linksFrom profileId Answer answerCount featuredAnswers ]
                                    , td [ class "portfolioContent" ] [ div [ class "contentType" ] <| requestAllContent linksFrom profileId Article articleCount featuredArticles ]
                                    ]
                                , tr [ class "contentTypeHeader" ]
                                    [ td [] [ b [] [ text "Podcasts" ] ]
                                    , td [] [ b [] [ text "Videos" ] ]
                                    ]
                                , tr []
                                    [ td [ class "portfolioContent" ] [ div [ class "contentType" ] <| requestAllContent linksFrom profileId Podcast podcastCount featuredPodcasts ]
                                    , td [ class "portfolioContent" ] [ div [ class "contentType" ] <| requestAllContent linksFrom profileId Video videoCount featuredVideos ]
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
        [ a [ class "allLinks", href <| urlText <| allContentUrl linksFrom profileId contentType ]
            [ text <| ("  view all " ++ toString count ++ " links  "), br [] [] ]
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
        a [ href <| urlText link.url, target "_blank" ] [ text <| formatTitle link, br [] [] ]
    else
        a [ class "featured", href <| urlText link.url, target "_blank" ] [ text <| formatTitle link, br [] [] ]


linksUI : List Link -> List (Html Msg)
linksUI links =
    links
        |> List.sortWith compareLinks
        |> List.take 5
        |> List.map (\link -> decorateIfFeatured link)
