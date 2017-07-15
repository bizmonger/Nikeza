module Controls.RecentProviderLinks exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)


-- Model


type Msg
    = None


formatLink : Link -> Html Msg
formatLink link =
    div []
        [ a [ href <| getUrl <| link.url ] [ i [] [ text <| getTitle link.title ] ]
        , br [] []
        ]


thumbnail : Provider -> Html Msg
thumbnail provider =
    let
        profile =
            provider.profile

        links =
            provider.recentLinks

        linksUI =
            div [] (links |> List.map formatLink)
    in
        div []
            [ table []
                [ tr []
                    [ td []
                        [ a [ href <| getUrl <| providerUrl profile.id ]
                            [ img [ src <| getUrl profile.imageUrl, width 75, height 75 ] [] ]
                        ]
                    , td [ class "bio" ]
                        [ td [] [ text <| getName provider.profile.firstName ++ " " ++ getName provider.profile.lastName ]
                        , linksUI
                        ]
                    ]
                ]
            ]
