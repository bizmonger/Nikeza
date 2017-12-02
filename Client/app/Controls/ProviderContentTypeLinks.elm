module Controls.ProviderContentTypeLinks exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onCheck, onInput)


type Msg
    = Featured ( Link, Bool )


update : Msg -> Provider -> Provider
update msg provider =
    case msg of
        Featured ( link, isFeatured ) ->
            let
                pendingLinks =
                    provider.portfolio

                setFeaturedLink l =
                    if l.title /= link.title then
                        l
                    else
                        { link | isFeatured = isFeatured }

                portfolio =
                    provider.portfolio

                setFeaturedLinks contentTypeLinks =
                    contentTypeLinks |> List.map setFeaturedLink
            in
                case link.contentType of
                    Article ->
                        { provider | portfolio = { pendingLinks | articles = setFeaturedLinks portfolio.articles } }

                    Video ->
                        { provider | portfolio = { pendingLinks | videos = setFeaturedLinks portfolio.videos } }

                    Podcast ->
                        { provider | portfolio = { pendingLinks | podcasts = setFeaturedLinks portfolio.podcasts } }

                    Answer ->
                        { provider | portfolio = { pendingLinks | answers = setFeaturedLinks portfolio.answers } }

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

        checkbox link =
            input [ type_ "checkbox", checked link.isFeatured, onCheck (\b -> Featured ( link, b )) ] []

        addCheckbox link element =
            div [] [ (checkbox link), element ]

        createLink link =
            let
                linkElement =
                    if isOwner && link.isFeatured then
                        span [ class "portfolioLink" ] [ a [ class featuredClass, href <| urlText link.url, target "_blank" ] [ text <| titleText link.title, br [] [] ] ]
                    else
                        span [ class "portfolioLink" ] [ a [ href <| urlText link.url, target "_blank" ] [ text <| titleText link.title, br [] [] ] ]
            in
                if isOwner then
                    addCheckbox link linkElement
                else
                    linkElement
    in
        table [ class "xx" ]
            [ tr []
                [ td [] [ h3 [ class "topicHeader" ] [ text <| "All " ++ (contentType |> contentTypeToText) ] ] ]
            , tr []
                [ td [] [ div [] <| List.map createLink posts ]
                ]
            ]
