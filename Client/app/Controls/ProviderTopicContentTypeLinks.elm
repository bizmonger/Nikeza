module Controls.ProviderTopicContentTypeLinks exposing (..)

import Domain.Core as Domain exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)


type Msg
    = None


view : Provider -> Topic -> ContentType -> Html Msg
view provider topic contentType =
    let
        ( topics, links ) =
            ( provider.topics, provider.portfolio )

        posts =
            links |> getLinks contentType |> List.filter (\l -> l.topics |> hasMatch topic)

        content =
            div [ class "mainContent" ]
                [ table []
                    [ tr []
                        [ td [] [ h3 [ class "topicHeader" ] [ text <| Domain.title topic contentType ] ] ]
                    , tr []
                        [ td [] [ div [] <| List.map (\link -> a [ href <| urlText link.url, target "_blank" ] [ text <| titleText link.title, br [] [] ]) posts ]
                        ]
                    ]
                ]
    in
        content
