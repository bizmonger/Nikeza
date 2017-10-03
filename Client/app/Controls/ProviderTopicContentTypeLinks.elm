module Controls.ProviderTopicContentTypeLinks exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)


-- UPDATE


type Msg
    = None



-- VIEW


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
                        [ td [] [ h3 [] [ text <| "All " ++ (contentType |> contentTypeToText) ] ] ]
                    , tr []
                        [ td [] [ div [] <| List.map (\link -> a [ href <| getUrl link.url, target "_blank" ] [ text <| getTitle link.title, br [] [] ]) posts ]
                        ]
                    ]
                ]
    in
        content
