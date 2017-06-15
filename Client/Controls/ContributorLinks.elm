module Controls.ContributorLinks exposing (..)

import Domain.Core exposing (..)
import Domain.Contributor as Contributor exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onCheck, onInput)


-- MODEL
-- UPDATE


type Msg
    = ToggleAll Bool
    | Toggle ( Topic, Bool )



-- VIEW


contributorPage : Contributor.Model -> Html Msg
contributorPage model =
    let
        ( profileId, topics ) =
            ( model.profile.id, model.profile.topics )

        allTopic =
            Topic "All"

        allFilter =
            div []
                [ input [ type_ "checkbox", checked model.showAll, onCheck (\b -> ToggleAll b) ] []
                , label [] [ text <| getTopic allTopic ]
                ]

        toCheckBoxState include topic =
            div []
                [ input [ type_ "checkbox", checked include, onCheck (\isChecked -> Toggle ( topic, isChecked )) ] []
                , label [] [ text <| getTopic topic ]
                ]
    in
        div []
            [ table []
                [ tr []
                    [ table []
                        [ tr []
                            [ td [] [ img [ src <| getUrl <| model.profile.imageUrl, width 100, height 100 ] [] ]

                            -- , td [] [ div [] <| allFilter :: (topics |> List.map (\t -> t |> toCheckBoxState model.showAll)) ]
                            , td [] [ div [] <| (topics |> List.map (\t -> t |> toCheckBoxState True)) ]
                            , table []
                                [ tr []
                                    [ td [] [ b [] [ text "Answers" ] ]
                                    , td [] [ b [] [ text "Articles" ] ]
                                    ]
                                , tr []
                                    [ td [] [ div [] <| contentUI profileId Answer model.answers ]
                                    , td [] [ div [] <| contentUI profileId Article model.articles ]
                                    ]
                                , tr []
                                    [ td [] [ b [] [ text "Podcasts" ] ]
                                    , td [] [ b [] [ text "Videos" ] ]
                                    ]
                                , tr []
                                    [ td [] [ div [] <| contentUI profileId Podcast model.podcasts ]
                                    , td [] [ div [] <| contentUI profileId Video model.videos ]
                                    ]
                                ]
                            ]
                        , tr [] [ td [] [ text <| getName model.profile.name ] ]
                        , tr [] [ td [] [ p [] [ text model.profile.bio ] ] ]
                        ]
                    ]
                ]
            ]


contentUI : Id -> ContentType -> List Link -> List (Html Msg)
contentUI profileId contentType links =
    List.append (linksUI links) [ a [ href <| getUrl <| moreContributorContentUrl profileId contentType ] [ text <| "all", br [] [] ] ]


linksUI : List Link -> List (Html Msg)
linksUI links =
    links
        |> List.take 5
        |> List.map (\link -> a [ href <| getUrl link.url ] [ text <| getTitle link.title, br [] [] ])
