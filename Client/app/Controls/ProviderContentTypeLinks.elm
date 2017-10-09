module Controls.ProviderContentTypeLinks exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onCheck, onInput)


type Msg
    = Toggle ( Topic, Bool )
    | Featured ( Link, Bool )


update : Msg -> Provider -> Provider
update msg provider =
    case msg of
        Toggle ( topic, include ) ->
            toggleFilter provider ( topic, include )

        Featured ( link, isFeatured ) ->
            let
                pendingLinks =
                    provider.portfolio

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
                                provider.portfolio.articles |> List.map setFeaturedLink
                        in
                            { provider | portfolio = { pendingLinks | articles = links } }

                    Video ->
                        let
                            links =
                                provider.portfolio.videos |> List.map setFeaturedLink
                        in
                            { provider | portfolio = { pendingLinks | videos = links } }

                    Podcast ->
                        let
                            links =
                                provider.portfolio.podcasts |> List.map setFeaturedLink
                        in
                            { provider | portfolio = { pendingLinks | podcasts = links } }

                    Answer ->
                        let
                            links =
                                provider.portfolio.answers |> List.map setFeaturedLink
                        in
                            { provider | portfolio = { pendingLinks | answers = links } }

                    All ->
                        provider

                    Unknown ->
                        provider



-- VIEW


view : Provider -> ContentType -> Bool -> Html Msg
view provider contentType isOwner =
    let
        ( topics, links, featuredClass ) =
            ( provider.topics, provider.portfolio, "featured" )

        posts =
            links |> getLinks contentType |> List.sortWith compareLinks

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
            input [ type_ "checkbox", checked link.isFeatured, onCheck (\b -> Featured ( link, b )) ] []

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


toCheckbox : Topic -> Html Msg
toCheckbox topic =
    div []
        [ input [ type_ "checkbox", checked True, onCheck (\b -> Toggle ( topic, b )) ] []
        , label [] [ text <| getTopic topic ]
        ]
