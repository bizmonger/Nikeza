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
            let
                updatedProvider =
                    toggleFilter provider ( topic, include )
            in
                { provider | filteredPortfolio = updatedProvider.filteredPortfolio }


view : Linksfrom -> Provider -> Html Msg
view linksFrom provider =
    let
        profileId =
            provider.profile.id

        toCheckBoxState include topic =
            div [ class "topicFilter" ]
                [ input [ type_ "checkbox", checked include, onCheck (\isChecked -> Toggle ( topic, isChecked )) ] []
                , label [ class "topicAdded" ] [ text <| topicText topic ]
                ]

        filtered =
            provider.filteredPortfolio

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
                            [ td [] [ input [ type_ "text", placeholder "search" ] [] ]
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
