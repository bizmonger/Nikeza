module Controls.RecentProviderLinks exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)


-- Model


type Msg
    = None


formatLink : Link -> Html Msg
formatLink link =
    a [ href <| getUrl <| link.url ] [ i [] [ text <| getTitle link.title ] ]


thumbnail : Provider -> Html Msg
thumbnail contentProvider =
    let
        profile =
            contentProvider.profile

        links =
            contentProvider.recentLinks

        nameAndLinks =
            div []
                [ label [] [ text <| (profile.firstName |> getName) ++ " " ++ (profile.lastName |> getName) ]
                , br [] []
                , div [] (links |> List.map formatLink)
                ]
    in
        div []
            [ table []
                [ tr []
                    [ td []
                        [ a [ href <| getUrl <| contentProviderUrl profile.id ]
                            [ img [ src <| getUrl profile.imageUrl, width 50, height 50 ] [] ]
                        ]
                    , td [] [ nameAndLinks ]
                    ]
                ]
            ]
