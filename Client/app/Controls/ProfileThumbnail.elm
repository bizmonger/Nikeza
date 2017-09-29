module Controls.ProfileThumbnail exposing (..)

import Settings exposing (..)
import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)


-- Model


type alias Model =
    Provider


type Msg
    = UpdateSubscription SubscriptionUpdate


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        UpdateSubscription update ->
            case update of
                Subscribe clientId providerId ->
                    case runtime.follow clientId providerId of
                        Ok _ ->
                            ( model, Cmd.none )

                        Err _ ->
                            ( model, Cmd.none )

                Unsubscribe clientId providerId ->
                    case runtime.follow clientId providerId of
                        Ok _ ->
                            ( model, Cmd.none )

                        Err _ ->
                            ( model, Cmd.none )


thumbnail : Maybe Provider -> Bool -> Provider -> Html Msg
thumbnail loggedInProvider showSubscribe provider =
    case loggedInProvider of
        Just loggedIn ->
            let
                profile =
                    provider.profile

                formatTopic topic =
                    a [ href <| getUrl <| providerTopicUrl (Just loggedIn.profile.id) profile.id topic ] [ i [] [ text <| getTopic topic ] ]

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
                    let
                        (Members followers) =
                            loggedIn.followers

                        isFollowing =
                            followers |> List.any (\p -> p.profile.id == loggedIn.profile.id)
                    in
                        if isFollowing then
                            "Unsubscribe"
                        else
                            "Follow"

                placeholder =
                    let
                        (Members followers) =
                            loggedIn.followers

                        isFollowing =
                            followers |> List.any (\p -> p.profile.id == loggedIn.profile.id)
                    in
                        if not isFollowing && showSubscribe then
                            button [ class "subscribeButton", onClick (UpdateSubscription <| Subscribe loggedIn.profile.id provider.profile.id) ] [ text "Follow" ]
                        else if isFollowing && showSubscribe then
                            button [ class "unsubscribeButton", onClick (UpdateSubscription <| Unsubscribe loggedIn.profile.id provider.profile.id) ] [ text "Unsubscribe" ]
                        else
                            div [] []
            in
                div []
                    [ table []
                        [ tr []
                            [ td []
                                [ a [ href <| getUrl <| providerUrl (Just loggedIn.profile.id) profile.id ]
                                    [ img [ src <| getUrl profile.imageUrl, width 65, height 65 ] [] ]
                                ]
                            , td [] [ nameAndTopics ]
                            ]
                        , placeholder
                        ]
                    ]

        Nothing ->
            div [] []
