module Controls.ProfileThumbnail exposing (..)

import Settings exposing (..)
import Domain.Core exposing (..)
import Services.Adapter exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import Http


type Msg
    = UpdateSubscription SubscriptionUpdate
    | SubscribeResponse (Result Http.Error JsonProvider)


update : Msg -> Provider -> ( Provider, Cmd Msg )
update msg provider =
    case msg of
        SubscribeResponse result ->
            case result of
                Ok jsonProvider ->
                    let
                        (Members providers) =
                            provider.subscriptions

                        subscriptions =
                            (jsonProvider |> toProvider) :: providers
                    in
                        ( { provider | subscriptions = Members subscriptions }, Cmd.none )

                Err _ ->
                    ( provider, Cmd.none )

        UpdateSubscription action ->
            case action of
                Subscribe clientId providerId ->
                    ( provider, (runtime.follow { subscriberId = clientId, providerId = providerId }) SubscribeResponse )

                Unsubscribe clientId providerId ->
                    ( provider, (runtime.unsubscribe { subscriberId = clientId, providerId = providerId }) SubscribeResponse )


thumbnail : Maybe Provider -> Bool -> Provider -> Html Msg
thumbnail loggedIn showSubscriptionState provider =
    let
        profile =
            provider.profile

        formatTopic topic =
            a [ href <| urlText <| providerTopicUrl (Just profile.id) profile.id topic ] [ i [] [ text <| topicText topic ] ]

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
                [ label [] [ text <| (profile.firstName |> nameText) ++ " " ++ (profile.lastName |> nameText) ]
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
                                    [ a [ href <| urlText <| providerUrl (Just user.profile.id) profile.id ]
                                        [ img [ src <| urlText profile.imageUrl, width 65, height 65 ] [] ]
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
                                [ a [ href <| urlText <| providerUrl Nothing profile.id ]
                                    [ img [ src <| urlText profile.imageUrl, width 65, height 65 ] [] ]
                                ]
                            , td [] [ nameAndTopics ]
                            ]
                        ]
                    ]
