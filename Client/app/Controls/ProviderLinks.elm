module Controls.ProviderLinks exposing (..)

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
        ( profileId, topics ) =
            ( provider.profile.id, provider.topics )

        toCheckBoxState include topic =
            div []
                [ input [ type_ "checkbox", checked include, onCheck (\isChecked -> Toggle ( topic, isChecked )) ] []
                , label [] [ text <| topicText topic ]
                ]

        filtered =
            provider.filteredPortfolio
    in
        div []
            [ table []
                [ tr []
                    [ table []
                        [ tr []
                            [ td [] [ div [] <| (topics |> List.map (\t -> t |> toCheckBoxState True)) ]
                            , table []
                                [ tr []
                                    [ td [] [ b [] [ text "Answers" ] ]
                                    , td [] [ b [] [ text "Articles" ] ]
                                    ]
                                , tr []
                                    [ td [] [ div [] <| requestAllContent linksFrom profileId Answer filtered.answers ]
                                    , td [] [ div [] <| requestAllContent linksFrom profileId Article filtered.articles ]
                                    ]
                                , tr []
                                    [ td [] [ b [] [ text "Podcasts" ] ]
                                    , td [] [ b [] [ text "Videos" ] ]
                                    ]
                                , tr []
                                    [ td [] [ div [] <| requestAllContent linksFrom profileId Podcast filtered.podcasts ]
                                    , td [] [ div [] <| requestAllContent linksFrom profileId Video filtered.videos ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]


requestAllContent : Linksfrom -> Id -> ContentType -> List Link -> List (Html Msg)
requestAllContent linksFrom profileId contentType links =
    let
        totalLinks =
            links |> List.length |> toString
    in
        List.append (linksUI links)
            [ a [ href <| urlText <| allContentUrl linksFrom profileId contentType ]
                [ text <| ("All (" ++ totalLinks ++ ") links"), br [] [] ]
            ]


decorateIfFeatured : Link -> Html Msg
decorateIfFeatured link =
    if not link.isFeatured then
        a [ href <| urlText link.url, target "_blank" ] [ text <| titleText link.title, br [] [] ]
    else
        a [ class "featured", href <| urlText link.url, target "_blank" ] [ text <| titleText link.title, br [] [] ]


linksUI : List Link -> List (Html Msg)
linksUI links =
    links
        |> List.sortWith compareLinks
        |> List.take 5
        |> List.map (\link -> decorateIfFeatured link)


toCheckbox : Topic -> Html Msg
toCheckbox topic =
    div []
        [ input [ type_ "checkbox", checked True, onCheck (\b -> Toggle ( topic, b )) ] []
        , label [] [ text <| topicText topic ]
        ]
