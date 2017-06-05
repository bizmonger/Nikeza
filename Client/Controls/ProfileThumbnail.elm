module Controls.ProfileThumbnail exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Settings exposing (..)


-- Model


type Msg
    = None


thumbnail : Profile -> Html Msg
thumbnail profile =
    let
        formatTopic topic =
            a [ href <| getUrl <| topicUrl runtime.topicUrl profile.id topic ] [ i [] [ text <| getTopic topic ] ]

        concatTopics topic1 topic2 =
            span []
                [ topic1
                , label [] [ text " " ]
                , topic2
                , label [] [ text " " ]
                ]

        topics =
            List.foldr concatTopics (div [] []) (profile.topics |> List.map formatTopic)

        topicsAndBio =
            div []
                [ label [] [ text (profile.name |> getName) ]
                , br [] []
                , topics
                ]
    in
        div []
            [ table []
                [ tr []
                    [ td []
                        [ a [ href <| getUrl <| runtime.contributorUrl profile.id ]
                            [ img [ src <| getUrl profile.imageUrl, width 50, height 50 ] [] ]
                        ]
                    , td [] [ topicsAndBio ]
                    ]
                ]

            --, label [] [ text (profile.name |> getName) ]
            ]
