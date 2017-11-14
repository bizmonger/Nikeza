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

                    Domain.Core.Featured ->
                        provider

                    Unknown ->
                        provider



-- VIEW


view : Provider -> ContentType -> Bool -> Html Msg
view provider contentType isOwner =
    let
        ( links, featuredClass ) =
            ( provider.portfolio, "featured" )

        posts =
            links |> getLinks contentType |> List.sortWith compareLinks

        createLink link =
            let
                linkElement =
                    if isOwner && link.isFeatured then
                        a [ class featuredClass, href <| urlText link.url, target "_blank" ] [ text <| titleText link.title, br [] [] ]
                    else
                        a [ href <| urlText link.url, target "_blank" ] [ text <| titleText link.title, br [] [] ]
            in
                linkElement
    in
        table [ class "xx" ]
            [ tr []
                [ td [] [ h3 [ class "topicHeader" ] [ text <| "All " ++ (contentType |> contentTypeToText) ] ] ]
            , tr []
                [ td [] [ div [] <| List.map createLink posts ]
                ]
            ]
