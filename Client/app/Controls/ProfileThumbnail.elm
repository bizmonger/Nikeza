module Controls.ProfileThumbnail exposing (..)

import Settings exposing (..)
import Domain.Core exposing (..)
import Services.Adapter exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import Http
import Tuple exposing (first, second)


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


organize : List Topic -> List Topic -> List Topic -> ( List Topic, List Topic )
organize group1 group2 remainingTopics =
    let
        next topics =
            topics |> List.reverse |> List.head

        sorted =
            remainingTopics |> List.sortBy (\t -> String.length t.name)
    in
        case sorted |> next of
            Nothing ->
                ( group1, group2 )

            Just longest ->
                let
                    updatedTopics =
                        sorted |> List.filter (\t -> t /= longest)
                in
                    case updatedTopics |> next of
                        Nothing ->
                            ( group1, longest :: group2 )

                        Just nextLongest ->
                            let
                                remaining =
                                    updatedTopics |> List.filter (\t -> t /= nextLongest)
                            in
                                if remaining |> List.isEmpty then
                                    ( longest :: group1, nextLongest :: group2 )
                                else
                                    remaining |> organize (longest :: group1) (nextLongest :: group2)


thumbnail : Maybe Provider -> Bool -> Provider -> Html Msg
thumbnail loggedIn showSubscriptionState provider =
    let
        profile =
            provider.profile

        formatTopic topic =
            a [ href <| urlText <| providerTopicUrl (Just profile.id) profile.id topic ] [ button [ class "topicsButton" ] [ text <| topicText topic ] ]

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

        group1 =
            List.foldr concatTopics
                (div [] [])
                (provider.topics |> organize [] [] |> first |> List.filterMap onFeaturedTopic)

        group2 =
            List.foldr concatTopics
                (div [] [])
                (provider.topics |> organize [] [] |> second |> List.filterMap onFeaturedTopic)

        nameAndTopics =
            div [ class "thumnnailDetails" ]
                [ label [ class "profileName" ] [ text <| (profile.firstName |> nameText) ++ " " ++ (profile.lastName |> nameText) ]
                , br [] []
                , group1
                , group2
                ]

        recentLinks =
            div [ class "centerDiv" ] (provider.recentLinks |> List.map (\l -> [ a [ href <| urlText l.url ] [ text <| titleText l.title ] ]) |> List.concat)
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
                    div [ class "landingThumbnail" ]
                        [ table []
                            [ tr []
                                [ td []
                                    [ a [ href <| urlText <| providerUrl (Just user.profile.id) profile.id ]
                                        [ img [ src <| urlText profile.imageUrl, width 65, height 65 ] [] ]
                                    , br [] []
                                    , label [ class "subscribed" ] [ text <| toString (List.length (getFollowers provider)) ++ " subscribers" ]
                                    ]
                                , td [] [ nameAndTopics ]
                                , td [ class "centertd" ] [ recentLinks ]
                                ]
                            , placeholder
                            ]
                        ]

            Nothing ->
                div [ class "landingThumbnail" ]
                    [ table []
                        [ tr []
                            [ td []
                                [ a [ href <| urlText <| providerUrl Nothing profile.id ]
                                    [ img [ src <| urlText profile.imageUrl, width 65, height 65 ] [] ]
                                , br [] []
                                , label [ class "subscribed" ] [ text <| toString (List.length (getFollowers provider)) ++ " subscribers" ]
                                ]
                            , td [] [ nameAndTopics ]
                            , td [ class "centertd" ] [ recentLinks ]
                            ]
                        ]
                    ]
