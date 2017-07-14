module Controls.ProfileThumbnail exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)


-- Model


type Msg
    = None


thumbnail : Provider -> Html Msg
thumbnail provider =
    let
        profile =
            provider.profile

        formatTopic topic =
            a [ href <| getUrl <| providerTopicUrl profile.id topic ] [ i [] [ text <| getTopic topic ] ]

        concatTopics topic1 topic2 =
            span []
                [ topic1
                , label [] [ text " " ]
                , topic2
                , label [] [ text " " ]
                ]

        topics =
            List.foldr concatTopics
                (div [] [])
                (provider.topics
                    |> List.filter (\t -> t.isFeatured)
                    |> List.map formatTopic
                )

        nameAndTopics =
            div []
                [ label [] [ text <| (profile.firstName |> getName) ++ " " ++ (profile.lastName |> getName) ]
                , br [] []
                , topics
                ]
    in
        div []
            [ table []
                [ tr []
                    [ td []
                        [ a [ href <| getUrl <| providerUrl profile.id ]
                            [ img [ src <| getUrl profile.imageUrl, width 50, height 50 ] [] ]
                        ]
                    , td [] [ nameAndTopics ]
                    ]
                ]
            ]
