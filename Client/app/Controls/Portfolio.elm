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

        ( answerCount, articleCount, podcastCount, videoCount ) =
            ( provider.portfolio |> getLinks Answer |> List.length
            , provider.portfolio |> getLinks Article |> List.length
            , provider.portfolio |> getLinks Podcast |> List.length
            , provider.portfolio |> getLinks Video |> List.length
            )
    in
        div [] [ label [] [ text <| toString (provider.portfolio |> getLinks All) ] ]



-- div []
--     [ table []
--         [ tr []
--             [ table []
--                 [ tr []
--                     -- [ td [] [ div [] <| (provider.topics |> List.map (\t -> t |> toCheckBoxState True)) ]
--                     [ td [] [ div [] <| (provider.portfolio |> getLinks All |> topicsFromLinks |> List.map (\t -> t |> toCheckBoxState True)) ]
--                     , table [ class "contentTable" ]
--                         [ tr [ class "contentTypeHeader" ]
--                             [ td [] [ b [] [ text "Answers" ] ]
--                             , td [] [ b [] [ text "Articles" ] ]
--                             ]
--                         , tr []
--                             [ td [ class "portfolioContent" ] [ div [ class "contentType" ] <| requestAllContent linksFrom profileId Answer filtered.answers answerCount ]
--                             , td [ class "portfolioContent" ] [ div [ class "contentType" ] <| requestAllContent linksFrom profileId Article filtered.articles articleCount ]
--                             ]
--                         , tr [ class "contentTypeHeader" ]
--                             [ td [] [ b [] [ text "Podcasts" ] ]
--                             , td [] [ b [] [ text "Videos" ] ]
--                             ]
--                         , tr []
--                             [ td [ class "portfolioContent" ] [ div [ class "contentType" ] <| requestAllContent linksFrom profileId Podcast filtered.podcasts podcastCount ]
--                             , td [ class "portfolioContent" ] [ div [ class "contentType" ] <| requestAllContent linksFrom profileId Video filtered.videos videoCount ]
--                             ]
--                         ]
--                     ]
--                 ]
--             ]
--         ]
--     ]


requestAllContent : Linksfrom -> Id -> ContentType -> List Link -> Int -> List (Html Msg)
requestAllContent linksFrom profileId contentType links count =
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
        a [ class "featured", href <| urlText link.url, target "_blank" ] [ text <| titleText link.title, br [] [] ]


linksUI : List Link -> List (Html Msg)
linksUI links =
    links
        |> List.sortWith compareLinks
        |> List.take 5
        |> List.map (\link -> decorateIfFeatured link)
