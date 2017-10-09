module Controls.RecentProviderLinks exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)


type Msg
    = None


formatLink : Link -> Html Msg
formatLink link =
    div []
        [ a [ href <| getUrl <| link.url ] [ i [] [ text <| getTitle link.title ] ]
        , br [] []
        ]


thumbnail : Id -> Provider -> Html Msg
thumbnail clientId provider =
    let
        ( profile, links ) =
            ( provider.profile, provider.recentLinks )

        linksUI =
            div [] (links |> List.map formatLink)
    in
        div []
            [ table []
                [ tr []
                    [ td []
                        [ a [ href <| getUrl <| providerUrl (Just clientId) profile.id ]
                            [ img [ src <| getUrl profile.imageUrl, width 75, height 75 ] [] ]
                        ]
                    , td [ class "bio" ]
                        [ td [] [ text <| getName provider.profile.firstName ++ " " ++ getName provider.profile.lastName ]
                        , linksUI
                        ]
                    ]
                ]
            ]
