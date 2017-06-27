module Controls.ContentProviderLinks exposing (..)

import Settings exposing (..)
import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onCheck, onInput)


-- MODEL


type alias Model =
    ContentProvider



-- UPDATE


type Msg
    = ToggleAll Bool
    | Toggle ( Topic, Bool )


update : Msg -> Model -> Model
update msg model =
    case msg of
        Toggle ( topic, include ) ->
            ( topic, include ) |> toggleFilter model

        ToggleAll include ->
            include |> toggleAllFilter model



-- VIEW


view : ContentProvider -> Html Msg
view model =
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
                                    [ td [] [ div [] <| notLoggedInRequestMoreContent profileId Answer links.answers ]
                                    , td [] [ div [] <| notLoggedInRequestMoreContent profileId Article links.articles ]
                                    ]
                                , tr []
                                    [ td [] [ b [] [ text "Podcasts" ] ]
                                    , td [] [ b [] [ text "Videos" ] ]
                                    ]
                                , tr []
                                    [ td [] [ div [] <| notLoggedInRequestMoreContent profileId Podcast links.podcasts ]
                                    , td [] [ div [] <| notLoggedInRequestMoreContent profileId Video links.videos ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]


notLoggedInRequestMoreContent : Id -> ContentType -> List Link -> List (Html Msg)
notLoggedInRequestMoreContent profileId contentType links =
    List.append (linksUI links) [ a [ href <| getUrl <| moreContentUrl profileId contentType ] [ text <| "all", br [] [] ] ]


linksUI : List Link -> List (Html Msg)
linksUI links =
    links
        |> List.take 5
        |> List.map (\link -> a [ href <| getUrl link.url ] [ text <| getTitle link.title, br [] [] ])


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
                | showAll = False
                , links =
                    { answers = links.answers |> toggleTopic Answer
                    , articles = links.articles |> toggleTopic Article
                    , videos = links.videos |> toggleTopic Video
                    , podcasts = links.podcasts |> toggleTopic Podcast
                    }
            }
    in
        newState


toggleAllFilter : ContentProvider -> Bool -> ContentProvider
toggleAllFilter model include =
    let
        profile =
            model.profile

        newState =
            if not include then
                { model | showAll = False, links = initLinks }
            else
                { model | showAll = True, links = profile.id |> runtime.links }
    in
        newState



-- REMOVE DUPLICATED FUNCTION ! ! !


toCheckbox : Topic -> Html Msg
toCheckbox topic =
    div []
        [ input [ type_ "checkbox", checked True, onCheck (\b -> Toggle ( topic, b )) ] []
        , label [] [ text <| getTopic topic ]
        ]
