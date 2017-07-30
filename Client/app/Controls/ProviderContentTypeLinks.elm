module Controls.ProviderContentTypeLinks exposing (..)

import Settings exposing (..)
import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onCheck, onInput)
import Dict exposing (..)


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
            let
                pendingLinks =
                    model.links

                removeLink linkToRemove links =
                    links |> List.filter (\l -> l.title /= linkToRemove.title)

                setFeaturedLink l =
                    if l.title /= link.title then
                        l
                    else
                        { link | isFeatured = isFeatured }
            in
                case link.contentType of
                    Article ->
                        let
                            links =
                                model.links.articles |> List.map setFeaturedLink
                        in
                            { model | links = { pendingLinks | articles = links } }

                    Video ->
                        let
                            links =
                                model.links.videos |> List.map setFeaturedLink
                        in
                            { model | links = { pendingLinks | videos = links } }

                    Podcast ->
                        let
                            links =
                                model.links.podcasts |> List.map setFeaturedLink
                        in
                            { model | links = { pendingLinks | podcasts = links } }

                    Answer ->
                        let
                            links =
                                model.links.answers |> List.map setFeaturedLink
                        in
                            { model | links = { pendingLinks | answers = links } }

                    All ->
                        model

                    Unknown ->
                        model



-- VIEW


view : Model -> ContentType -> Bool -> Html Msg
view model contentType isOwner =
    let
        ( topics, links, featuredClass ) =
            ( model.topics, model.links, "featured" )

        posts =
            links |> getPosts contentType |> List.sortWith compareLinks

        createLink link =
            let
                linkElement =
                    if isOwner && link.isFeatured then
                        a [ class featuredClass, href <| getUrl link.url, target "_blank" ] [ text <| getTitle link.title, br [] [] ]
                    else
                        a [ href <| getUrl link.url, target "_blank" ] [ text <| getTitle link.title, br [] [] ]
            in
                if isOwner then
                    addCheckbox link linkElement
                else
                    linkElement

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
                [ td [] [ h3 [] [ text <| "All " ++ (contentType |> contentTypeToText) ] ] ]
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
