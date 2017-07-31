module Controls.ProviderLinks exposing (..)

import Settings exposing (..)
import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onCheck, onInput)


-- MODEL


type alias Model =
    Provider



-- UPDATE


type Msg
    = Toggle ( Topic, Bool )


update : Msg -> Model -> Model
update msg model =
    case msg of
        Toggle ( topic, include ) ->
            ( topic, include ) |> toggleFilter model



-- VIEW


view : Linksfrom -> Provider -> Html Msg
view linksFrom model =
    let
        ( profileId, topics ) =
            ( model.profile.id, model.topics )

        toCheckBoxState include topic =
            div []
                [ input [ type_ "checkbox", checked include, onCheck (\isChecked -> Toggle ( topic, isChecked )) ] []
                , label [] [ text <| getTopic topic ]
                ]

        links =
            model.links
    in
        div []
            [ table []
                [ tr []
                    [ table []
                        [ tr []
                            [ td [] [ div [] <| (topics |> List.map (\t -> t |> toCheckBoxState True)) ]
                            , table []
                                [ tr []
                                    [ td [] [ b [] [ text "Answers" ] ]
                                    , td [] [ b [] [ text "Articles" ] ]
                                    ]
                                , tr []
                                    [ td [] [ div [] <| requestAllContent linksFrom profileId Answer links.answers ]
                                    , td [] [ div [] <| requestAllContent linksFrom profileId Article links.articles ]
                                    ]
                                , tr []
                                    [ td [] [ b [] [ text "Podcasts" ] ]
                                    , td [] [ b [] [ text "Videos" ] ]
                                    ]
                                , tr []
                                    [ td [] [ div [] <| requestAllContent linksFrom profileId Podcast links.podcasts ]
                                    , td [] [ div [] <| requestAllContent linksFrom profileId Video links.videos ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]


requestAllContent : Linksfrom -> Id -> ContentType -> List Link -> List (Html Msg)
requestAllContent linksFrom profileId contentType links =
    let
        totalLinks =
            links |> List.length |> toString
    in
        List.append (linksUI links)
            [ a [ href <| getUrl <| allContentUrl linksFrom profileId contentType ]
                [ text <| ("All (" ++ totalLinks ++ ") links"), br [] [] ]
            ]


decorate : Link -> Html Msg
decorate link =
    if not link.isFeatured then
        a [ href <| getUrl link.url, target "_blank" ] [ text <| getTitle link.title, br [] [] ]
    else
        b [] [ a [ class "featured", href <| getUrl link.url, target "_blank" ] [ text <| getTitle link.title, br [] [] ] ]


linksUI : List Link -> List (Html Msg)
linksUI links =
    links
        |> List.take 5
        |> List.map (\link -> decorate link)


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



-- REMOVE DUPLICATED FUNCTION ! ! !


toCheckbox : Topic -> Html Msg
toCheckbox topic =
    div []
        [ input [ type_ "checkbox", checked True, onCheck (\b -> Toggle ( topic, b )) ] []
        , label [] [ text <| getTopic topic ]
        ]
