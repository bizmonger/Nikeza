module Controls.RecentProviderLinks exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)


type Msg
    = None


formatLink : Link -> Html Msg
formatLink link =
    div []
        [ a [ href <| urlText <| link.url ] [ label [ class "recentLink" ] [ text <| titleText link.title ] ]
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
        div [ class "landingThumbnail" ]
            [ table []
                [ tr []
                    [ td []
                        [ a [ href <| urlText <| providerUrl (Just clientId) profile.id ]
                            [ img [ src <| urlText profile.imageUrl, width 40, height 40 ] [] ]
                        ]
                    , td []
                        [ div [ class "recentThumbnail" ]
                            [ b [ class "profileName" ] [ text <| nameText provider.profile.firstName ++ " " ++ nameText provider.profile.lastName ]
                            , linksUI
                            ]
                        ]
                    ]
                ]
            ]
