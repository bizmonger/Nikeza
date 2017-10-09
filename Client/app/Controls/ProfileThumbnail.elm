module Controls.ProfileThumbnail exposing (..)

import Settings exposing (..)
import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)


type Msg
    = UpdateSubscription SubscriptionUpdate


update : Msg -> Provider -> ( Provider, Cmd Msg )
update msg provider =
    case msg of
        UpdateSubscription update ->
            case update of
                Subscribe clientId providerId ->
                    case runtime.follow clientId providerId of
                        Ok _ ->
                            ( provider, Cmd.none )

                        Err _ ->
                            ( provider, Cmd.none )

                Unsubscribe clientId providerId ->
                    case runtime.follow clientId providerId of
                        Ok _ ->
                            ( provider, Cmd.none )

                        Err _ ->
                            ( provider, Cmd.none )


thumbnail : Maybe Provider -> Bool -> Provider -> Html Msg
thumbnail loggedIn showSubscriptionState provider =
    let
        profile =
            provider.profile

        formatTopic topic =
            a [ href <| getUrl <| providerTopicUrl (Just profile.id) profile.id topic ] [ i [] [ text <| getTopic topic ] ]

        concatTopics topic1 topic2 =
            span []
                [ topic1
                , label [] [ text " " ]
                , topic2
                , label [] [ text " " ]
                ]

        onFeaturedTopic topic =
            if topic.isFeatured then
                Just (formatTopic topic)
            else
                Nothing

        topics =
            List.foldr concatTopics
                (div [] [])
                (provider.topics |> List.filterMap onFeaturedTopic)

        nameAndTopics =
            div []
                [ label [] [ text <| (profile.firstName |> getName) ++ " " ++ (profile.lastName |> getName) ]
                , br [] []
                , topics
                ]
    in
        case loggedIn of
            Just user ->
                let
                    (Members mySubscriptions) =
                        user.subscriptions

                    alreadySubscribed =
                        mySubscriptions |> List.any (\subscription -> subscription.profile.id == profile.id)

                    subscriptionText =
                        if alreadySubscribed then
                            "Unsubscribe"
                        else
                            "Follow"

                    placeholder =
                        if not alreadySubscribed && showSubscriptionState then
                            button [ class "subscribeButton", onClick (UpdateSubscription <| Subscribe user.profile.id provider.profile.id) ] [ text "Follow" ]
                        else if alreadySubscribed && showSubscriptionState then
                            button [ class "unsubscribeButton", onClick (UpdateSubscription <| Unsubscribe user.profile.id provider.profile.id) ] [ text "Unsubscribe" ]
                        else
                            div [] []
                in
                    div []
                        [ table []
                            [ tr []
                                [ td []
                                    [ a [ href <| getUrl <| providerUrl (Just user.profile.id) profile.id ]
                                        [ img [ src <| getUrl profile.imageUrl, width 65, height 65 ] [] ]
                                    ]
                                , td [] [ nameAndTopics ]
                                ]
                            , placeholder
                            ]
                        ]

            Nothing ->
                div []
                    [ table []
                        [ tr []
                            [ td []
                                [ a [ href <| getUrl <| providerUrl Nothing profile.id ]
                                    [ img [ src <| getUrl profile.imageUrl, width 65, height 65 ] [] ]
                                ]
                            , td [] [ nameAndTopics ]
                            ]
                        ]
                    ]
