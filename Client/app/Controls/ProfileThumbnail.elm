module Controls.ProfileThumbnail exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)


-- Model


type Msg
    = None


thumbnail : Maybe Id -> Bool -> Provider -> Html Msg
thumbnail profileId showSubscribe provider =
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

        subscriptionText =
            case profileId of
                Just id ->
                    let
                        (Subscribers followers) =
                            provider.followers provider.profile.id

                        isFollowing =
                            followers |> List.any (\p -> p.profile.id == id)
                    in
                        if isFollowing then
                            "Unsubscribe"
                        else
                            "Subscribe"

                Nothing ->
                    "Subscribe"

        placeholder =
            case profileId of
                Just id ->
                    let
                        (Subscribers followers) =
                            provider.followers provider.profile.id

                        isFollowing =
                            followers |> List.any (\p -> p.profile.id == id)
                    in
                        if not isFollowing && showSubscribe then
                            button [ class "subscribeButton" ] [ text subscriptionText ]
                        else if isFollowing && showSubscribe then
                            button [ class "unsubscribeButton" ] [ text subscriptionText ]
                        else
                            div [] []

                Nothing ->
                    div [] []
    in
        div []
            [ table []
                [ tr []
                    [ td []
                        [ a [ href <| getUrl <| providerUrl profile.id ]
                            [ img [ src <| getUrl profile.imageUrl, width 65, height 65 ] [] ]
                        ]
                    , td [] [ nameAndTopics ]
                    ]
                , placeholder
                ]
            ]
