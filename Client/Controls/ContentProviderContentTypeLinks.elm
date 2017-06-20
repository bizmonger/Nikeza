module Controls.ContentProviderContentTypeLinks exposing (..)

import Settings exposing (..)
import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onCheck, onInput)


type alias Model =
    ContentProvider



-- UPDATE


type Msg
    = ToggleAll Bool
    | Toggle ( Topic, Bool )


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        Toggle ( topic, include ) ->
            ( topic, include ) |> toggleFilter model

        ToggleAll include ->
            include |> toggleAllFilter model



-- VIEW


view : Model -> ContentType -> Html Msg
view model contentType =
    let
        ( topics, links ) =
            ( model.topics, model.links )

        posts =
            case contentType of
                Answer ->
                    links.answers

                Article ->
                    links.articles

                Podcast ->
                    links.podcasts

                Video ->
                    links.videos

                Unknown ->
                    []

                All ->
                    []
    in
        div []
            [ h2 [] [ text <| "All " ++ (contentType |> contentTypeToText) ]
            , table []
                [ tr []
                    [ td [] [ div [] (topics |> List.map toCheckbox) ]
                    , td [] [ div [] <| List.map (\link -> a [ href <| getUrl link.url ] [ text <| getTitle link.title, br [] [] ]) posts ]
                    ]
                ]
            ]



-- REMOVE DUPLICATED FUNCTION ! ! !


toCheckbox : Topic -> Html Msg
toCheckbox topic =
    div []
        [ input [ type_ "checkbox", checked True, onCheck (\b -> Toggle ( topic, b )) ] []
        , label [] [ text <| getTopic topic ]
        ]


toggleFilter : Model -> ( Topic, Bool ) -> ( Model, Cmd Msg )
toggleFilter model ( topic, include ) =
    let
        toggleTopic contentType links =
            if include then
                List.append (model.profile.id |> runtime.topicLinks topic contentType) links
            else
                links |> List.filter (\link -> not (link.topics |> List.member topic))

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
        ( newState, Cmd.none )


toggleAllFilter : Model -> Bool -> ( Model, Cmd Msg )
toggleAllFilter model include =
    let
        contentProvider =
            model

        newState =
            if not include then
                { contentProvider | showAll = False, links = initLinks }
            else
                { contentProvider | showAll = True, links = contentProvider.profile.id |> runtime.links }
    in
        ( newState, Cmd.none )
