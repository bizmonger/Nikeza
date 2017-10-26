module Controls.RecentProviderLinks exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)


type Msg
    = None


formatLink : Link -> Html Msg
formatLink link =
    div []
        [ a [ href <| urlText <| link.url ] [ i [] [ text <| titleText link.title ] ]
        , br [] []
        ]


thumbnail : Id -> Provider -> Html Msg
thumbnail clientId provider =
    let
        ( profile, links ) =
            ( provider.profile, [] {- Todo: get recent links based on what loggedIn user hasn't observed -} )

        linksUI =
            div [] (links |> List.map formatLink)
    in
        div []
            [ table []
                [ tr []
                    [ td []
                        [ a [ href <| urlText <| providerUrl (Just clientId) profile.id ]
                            [ img [ src <| urlText profile.imageUrl, width 75, height 75 ] [] ]
                        ]
                    , td [ class "bio" ]
                        [ td [] [ text <| nameText provider.profile.firstName ++ " " ++ nameText provider.profile.lastName ]
                        , linksUI
                        ]
                    ]
                ]
            ]
