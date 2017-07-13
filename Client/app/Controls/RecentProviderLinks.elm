module Controls.RecentProviderLinks exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)


-- Model


type Msg
    = None


thumbnail : ContentProvider -> Html Msg
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
                , links
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
