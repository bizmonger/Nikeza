module Controls.ProviderContentTypeLinks exposing (..)

import Settings exposing (..)
import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onCheck, onInput)


type alias Model =
    Provider



-- UPDATE


type Msg
    = Toggle ( Topic, Bool )
    | Featured ( Link, Bool )


update : Msg -> Model -> Model
update msg model =
    case msg of
        Toggle ( topic, include ) ->
            ( topic, include ) |> toggleFilter model

        Featured ( link, isFeatured ) ->
            model



-- VIEW


view : Model -> ContentType -> Html Msg
view model contentType =
    let
        ( topics, links ) =
            ( model.topics, model.links )

        posts =
            links |> getPosts contentType

        featuredClass =
            "featured"

        createLink link =
            let
                linkElement =
                    if link.isFeatured then
                        a [ class featuredClass, href <| getUrl link.url, target "_blank" ] [ text <| getTitle link.title, br [] [] ]
                    else
                        a [ href <| getUrl link.url, target "_blank" ] [ text <| getTitle link.title, br [] [] ]
            in
                addCheckbox link linkElement

        checkbox link =
            input [ type_ "checkbox", checked False, onCheck (\b -> Featured ( link, b )) ] []

        addCheckbox link element =
            div []
                [ (checkbox link)
                , element
                ]
    in
        table []
            [ tr []
                [ td [] [ h2 [] [ text <| "All " ++ (contentType |> contentTypeToText) ] ] ]
            , tr []
                [ td [] [ div [] (topics |> List.map toCheckbox) ]
                , td [] [ div [] <| List.map createLink posts ]
                ]
            ]



-- REMOVE DUPLICATED FUNCTION ! ! !


toCheckbox : Topic -> Html Msg
toCheckbox topic =
    div []
        [ input [ type_ "checkbox", checked True, onCheck (\b -> Toggle ( topic, b )) ] []
        , label [] [ text <| getTopic topic ]
        ]


toggleFilter : Model -> ( Topic, Bool ) -> Model
toggleFilter model ( topic, include ) =
    let
        toggleTopic contentType links =
            if include then
                List.append (model.profile.id |> runtime.topicLinks topic contentType) links
            else
                links |> List.filter (\link -> not (link.topics |> hasMatch topic))

        links =
            model.links

        newState =
            { model
                | links =
                    { answers = links.answers |> toggleTopic Answer
                    , articles = links.articles |> toggleTopic Article
                    , videos = links.videos |> toggleTopic Video
                    , podcasts = links.podcasts |> toggleTopic Podcast
                    }
            }
    in
        newState
