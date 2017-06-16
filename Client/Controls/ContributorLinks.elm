module Controls.ContributorLinks exposing (..)

import Settings exposing (..)
import Domain.Core exposing (..)
import Domain.Contributor as Contributor exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onCheck, onInput)


-- MODEL


type alias Model =
    Contributor.Model



-- UPDATE


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        Toggle ( topic, include ) ->
            ( topic, include ) |> toggleFilter model

        ToggleAll include ->
            include |> toggleAllFilter model


type Msg
    = ToggleAll Bool
    | Toggle ( Topic, Bool )



-- VIEW


view : Contributor.Model -> Html Msg
view model =
    let
        ( profileId, topics ) =
            ( model.profile.id, model.profile.topics )

        allTopic =
            Topic "All"

        allFilter =
            div []
                [ input [ type_ "checkbox", checked model.showAll, onCheck (\b -> ToggleAll b) ] []
                , label [] [ text <| getTopic allTopic ]
                ]

        toCheckBoxState include topic =
            div []
                [ input [ type_ "checkbox", checked include, onCheck (\isChecked -> Toggle ( topic, isChecked )) ] []
                , label [] [ text <| getTopic topic ]
                ]
    in
        div []
            [ table []
                [ tr []
                    [ table []
                        [ tr []
                            [ td [] [ img [ src <| getUrl <| model.profile.imageUrl, width 100, height 100 ] [] ]

                            -- , td [] [ div [] <| allFilter :: (topics |> List.map (\t -> t |> toCheckBoxState model.showAll)) ]
                            , td [] [ div [] <| (topics |> List.map (\t -> t |> toCheckBoxState True)) ]
                            , table []
                                [ tr []
                                    [ td [] [ b [] [ text "Answers" ] ]
                                    , td [] [ b [] [ text "Articles" ] ]
                                    ]
                                , tr []
                                    [ td [] [ div [] <| contentUI profileId Answer model.answers ]
                                    , td [] [ div [] <| contentUI profileId Article model.articles ]
                                    ]
                                , tr []
                                    [ td [] [ b [] [ text "Podcasts" ] ]
                                    , td [] [ b [] [ text "Videos" ] ]
                                    ]
                                , tr []
                                    [ td [] [ div [] <| contentUI profileId Podcast model.podcasts ]
                                    , td [] [ div [] <| contentUI profileId Video model.videos ]
                                    ]
                                ]
                            ]
                        , tr [] [ td [] [ text <| getName model.profile.name ] ]
                        , tr [] [ td [] [ p [] [ text model.profile.bio ] ] ]
                        ]
                    ]
                ]
            ]


contentUI : Id -> ContentType -> List Link -> List (Html Msg)
contentUI profileId contentType links =
    List.append (linksUI links) [ a [ href <| getUrl <| moreContributorContentUrl profileId contentType ] [ text <| "all", br [] [] ] ]


linksUI : List Link -> List (Html Msg)
linksUI links =
    links
        |> List.take 5
        |> List.map (\link -> a [ href <| getUrl link.url ] [ text <| getTitle link.title, br [] [] ])


toggleFilter : Contributor.Model -> ( Topic, Bool ) -> ( Contributor.Model, Cmd Msg )
toggleFilter model ( topic, include ) =
    let
        toggleTopic contentType links =
            if include then
                List.append (model.profile.id |> runtime.topicLinks topic contentType) links
            else
                links |> List.filter (\l -> not (l.topics |> List.member topic))

        newState =
            { model
                | showAll = False
                , answers = model.answers |> toggleTopic Answer
                , articles = model.articles |> toggleTopic Article
                , videos = model.videos |> toggleTopic Video
                , podcasts = model.podcasts |> toggleTopic Podcast
            }
    in
        ( newState, Cmd.none )


toggleAllFilter : Contributor.Model -> Bool -> ( Contributor.Model, Cmd Msg )
toggleAllFilter model include =
    let
        profile =
            model.profile

        newState =
            if not include then
                { model | showAll = False, answers = [], articles = [], videos = [], podcasts = [] }
            else
                { model
                    | showAll = True
                    , answers = profile.id |> runtime.links Answer
                    , articles = profile.id |> runtime.links Article
                    , videos = profile.id |> runtime.links Video
                    , podcasts = profile.id |> runtime.links Podcast
                }
    in
        ( newState, Cmd.none )


toCheckbox : Topic -> Html Msg
toCheckbox topic =
    div []
        [ input [ type_ "checkbox", checked True, onCheck (\b -> Toggle ( topic, b )) ] []
        , label [] [ text <| getTopic topic ]
        ]
