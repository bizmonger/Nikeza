module Controls.ProviderTopicContentTypeLinks exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)


type alias Model =
    Provider



-- UPDATE


type Msg
    = None



-- VIEW


view : Model -> Topic -> ContentType -> Html Msg
view model topic contentType =
    let
        ( topics, links ) =
            ( model.topics, model.links )

        posts =
            links |> getPosts contentType |> List.filter (\l -> hasMatch topic l.topics)
    in
        table []
            [ tr []
                [ td [] [ h3 [] [ text <| "All " ++ (contentType |> contentTypeToText) ] ] ]
            , tr []
                [ td [] [ div [] <| List.map (\link -> a [ href <| getUrl link.url, target "_blank" ] [ text <| getTitle link.title, br [] [] ]) posts ]
                ]
            ]
