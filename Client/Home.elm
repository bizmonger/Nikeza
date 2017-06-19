module Home exposing (..)

import Settings exposing (runtime)
import Domain.Core as Domain exposing (..)
import Domain.Contributor as Contributor exposing (..)
import Controls.Login as Login exposing (..)
import Controls.ProfileThumbnail as ProfileThumbnail exposing (..)
import Controls.AddConnection as AddConnection exposing (..)
import Controls.NewLinks as NewLinks exposing (..)
import Controls.ContributorLinks as ContributorLinks exposing (..)
import Controls.ContributorContentTypeLinks as ContributorContentTypeLinks exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onCheck, onInput)
import Navigation exposing (..)
import String exposing (..)


-- elm-live Home.elm --open --output=home.js
-- elm-make Home.elm --output=home.html
-- elm-package install elm-lang/navigation


main =
    Navigation.program UrlChange
        { init = init
        , view = view
        , update = update
        , subscriptions = (\_ -> Sub.none)
        }



-- MODEL


type alias Model =
    { currentRoute : Navigation.Location
    , login : Login.Model
    , portal : Portal
    , contributors : List Contributor
    , selectedContributor : Contributor
    }


init : Navigation.Location -> ( Model, Cmd Msg )
init location =
    let
        contributor =
            case tokenizeUrl location.hash of
                [ "contributor", id ] ->
                    case runtime.contributor <| Id id of
                        Just c ->
                            c

                        Nothing ->
                            initContributor

                _ ->
                    initContributor
    in
        ( { currentRoute = location
          , login = Login.model
          , portal = initPortal
          , contributors = runtime.contributors
          , selectedContributor = contributor
          }
        , Cmd.none
        )



-- UPDATE


type Msg
    = UrlChange Navigation.Location
    | OnLogin Login.Msg
    | ProfileThumbnail ProfileThumbnail.Msg
    | NewConnection AddConnection.Msg
    | Remove Connection
    | ViewConnections
    | AddNewLink
    | ViewLinks
    | NewLink NewLinks.Msg
    | ContributorLinksAction ContributorLinks.Msg
    | ContributorContentTypeLinksAction ContributorContentTypeLinks.Msg
    | Search String
    | Register


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        UrlChange location ->
            location |> navigate msg model

        OnLogin subMsg ->
            onLogin model subMsg

        Search "" ->
            ( { model | contributors = runtime.contributors }, Cmd.none )

        Search text ->
            text |> matchContributors model

        Register ->
            ( model, Cmd.none )

        ProfileThumbnail subMsg ->
            ( model, Cmd.none )

        ViewConnections ->
            let
                pendingPortal =
                    model.portal
            in
                ( { model | portal = { pendingPortal | requested = Domain.ViewConnections } }, Cmd.none )

        AddNewLink ->
            let
                pendingPortal =
                    model.portal
            in
                ( { model | portal = { pendingPortal | requested = Domain.AddLink } }, Cmd.none )

        ViewLinks ->
            let
                pendingPortal =
                    model.portal
            in
                ( { model | portal = { pendingPortal | requested = Domain.ViewLinks } }, Cmd.none )

        NewConnection subMsg ->
            onNewConnection subMsg model

        Remove connection ->
            onRemove model connection

        NewLink subMsg ->
            onNewLink subMsg model

        ContributorLinksAction subMsg ->
            case subMsg of
                ContributorLinks.ToggleAll _ ->
                    let
                        ( contributor, _ ) =
                            ContributorLinks.update subMsg model.selectedContributor
                    in
                        ( { model | selectedContributor = contributor }, Cmd.none )

                ContributorLinks.Toggle _ ->
                    let
                        ( contributor, _ ) =
                            ContributorLinks.update subMsg model.selectedContributor
                    in
                        ( { model | selectedContributor = contributor }, Cmd.none )

        ContributorContentTypeLinksAction subMsg ->
            case subMsg of
                ContributorContentTypeLinks.ToggleAll _ ->
                    let
                        ( contributor, _ ) =
                            ContributorContentTypeLinks.update subMsg model.selectedContributor
                    in
                        ( { model | selectedContributor = contributor }, Cmd.none )

                ContributorContentTypeLinks.Toggle _ ->
                    let
                        ( contributor, _ ) =
                            ContributorContentTypeLinks.update subMsg model.selectedContributor
                    in
                        ( { model | selectedContributor = contributor }, Cmd.none )


onRemove : Model -> Connection -> ( Model, Cmd Msg )
onRemove model connection =
    let
        contributor =
            model.portal.contributor

        profile =
            contributor.profile

        connectionsLeft =
            profile.connections |> List.filter (\c -> c /= connection)

        updatedProfile =
            { profile | connections = connectionsLeft }

        updatedContributor =
            { contributor | profile = updatedProfile }

        portal =
            { contributor = updatedContributor
            , requested = Domain.ViewLinks
            , newConnection = initConnection
            , newLinks = model.portal.newLinks
            }

        newState =
            { model | portal = portal }
    in
        ( newState, Cmd.none )


onNewLink : NewLinks.Msg -> Model -> ( Model, Cmd Msg )
onNewLink subMsg model =
    let
        pendingPortal =
            model.portal

        contributor =
            model.portal.contributor

        newLinks =
            NewLinks.update subMsg pendingPortal.newLinks

        portal =
            { pendingPortal | newLinks = newLinks }
    in
        case subMsg of
            NewLinks.InputTitle _ ->
                ( { model | portal = portal }, Cmd.none )

            NewLinks.InputUrl _ ->
                ( { model | portal = portal }, Cmd.none )

            NewLinks.InputTopic _ ->
                ( { model | portal = portal }, Cmd.none )

            NewLinks.RemoveTopic _ ->
                ( { model | portal = portal }, Cmd.none )

            NewLinks.AssociateTopic _ ->
                ( { model | portal = portal }, Cmd.none )

            NewLinks.InputContentType _ ->
                ( { model | portal = portal }, Cmd.none )

            NewLinks.AddLink v ->
                let
                    updatedLinks =
                        { newLinks | canAdd = True, added = v.current.base :: v.added }

                    updatedPortal =
                        { portal | newLinks = updatedLinks }
                in
                    ( { model | portal = updatedPortal }
                    , Cmd.none
                    )


onNewConnection : AddConnection.Msg -> Model -> ( Model, Cmd Msg )
onNewConnection subMsg model =
    let
        pendingPortal =
            model.portal

        contributor =
            model.portal.contributor

        connection =
            AddConnection.update subMsg pendingPortal.newConnection

        portal =
            { pendingPortal | newConnection = connection }
    in
        case subMsg of
            AddConnection.InputUsername _ ->
                ( { model | portal = portal }, Cmd.none )

            AddConnection.InputPlatform _ ->
                ( { model | portal = portal }, Cmd.none )

            AddConnection.Submit _ ->
                let
                    pendingProfile =
                        contributor.profile

                    updatedProfile =
                        { pendingProfile | connections = connection :: contributor.profile.connections }

                    updatedPortal =
                        { portal | contributor = { contributor | profile = updatedProfile } }
                in
                    ( { model | portal = updatedPortal }, Cmd.none )


matchContributors : Model -> String -> ( Model, Cmd Msg )
matchContributors model matchValue =
    let
        onName contributor =
            contributor.profile.name
                |> getName
                |> toLower
                |> contains (matchValue |> toLower)

        filtered =
            runtime.contributors |> List.filter onName
    in
        ( { model | contributors = filtered }, Cmd.none )


onLogin : Model -> Login.Msg -> ( Model, Cmd Msg )
onLogin model subMsg =
    let
        pendingPortal =
            model.portal
    in
        case subMsg of
            Login.Attempt _ ->
                let
                    login =
                        Login.update subMsg model.login

                    latest =
                        runtime.tryLogin login

                    contributorResult =
                        runtime.contributor <| runtime.usernameToId latest.username

                    newState =
                        case contributorResult of
                            Just c ->
                                { model
                                    | login = latest
                                    , portal = { pendingPortal | contributor = c }
                                }

                            Nothing ->
                                { model | login = latest }
                in
                    if newState.login.loggedIn then
                        ( newState, Navigation.load <| "/#/" ++ getId newState.portal.contributor.profile.id ++ "/dashboard" )
                    else
                        ( newState, Cmd.none )

            Login.UserInput _ ->
                ( { model | login = Login.update subMsg model.login }, Cmd.none )

            Login.PasswordInput _ ->
                ( { model | login = Login.update subMsg model.login }, Cmd.none )



-- VIEW


view : Model -> Html Msg
view model =
    case model.currentRoute.hash |> tokenizeUrl of
        [] ->
            homePage model

        [ "home" ] ->
            homePage model

        [ "contributor", id ] ->
            case runtime.contributor <| Id id of
                Just _ ->
                    table []
                        [ tr []
                            [ td [] [ img [ src <| getUrl <| model.selectedContributor.profile.imageUrl, width 100, height 100 ] [] ]
                            , td [] [ Html.map ContributorLinksAction <| ContributorLinks.view model.selectedContributor ]
                            ]
                        , tr [] [ td [] [ text <| getName model.selectedContributor.profile.name ] ]
                        , tr [] [ td [] [ p [] [ text model.selectedContributor.profile.bio ] ] ]
                        ]

                Nothing ->
                    notFoundPage

        [ "contributor", id, topic ] ->
            case runtime.contributor <| Id id of
                Just _ ->
                    contributorTopicPage model.selectedContributor

                Nothing ->
                    notFoundPage

        [ "contributor", id, "all", contentType ] ->
            case runtime.contributor <| Id id of
                Just c ->
                    let
                        contributorContentType =
                            { contributor = c
                            , contentType = contentType |> toContentType
                            }
                    in
                        table []
                            [ tr []
                                [ td [] [ img [ src <| getUrl <| model.selectedContributor.profile.imageUrl, width 100, height 100 ] [] ]
                                , td [] [ Html.map ContributorContentTypeLinksAction <| ContributorContentTypeLinks.view contributorContentType.contributor contributorContentType.contentType ]
                                ]
                            , tr [] [ td [] [ text <| getName model.selectedContributor.profile.name ] ]
                            , tr [] [ td [] [ p [] [ text model.selectedContributor.profile.bio ] ] ]
                            ]

                Nothing ->
                    notFoundPage

        [ "contributor", id, topic, "all", contentType ] ->
            case runtime.contributor <| Id id of
                Just _ ->
                    contributorTopicContentTypePage (Topic topic) (toContentType contentType) model.portal.contributor

                Nothing ->
                    notFoundPage

        [ id, "dashboard" ] ->
            dashboardPage model

        _ ->
            notFoundPage


homePage : Model -> Html Msg
homePage model =
    let
        loginUI : Model -> Html Msg
        loginUI model =
            let
                ( loggedIn, welcome, signout ) =
                    ( model.login.loggedIn
                    , p [] [ text <| "Welcome " ++ model.login.username ++ "!" ]
                    , a [ href "" ] [ label [] [ text "Signout" ] ]
                    )
            in
                if (not loggedIn) then
                    Html.map OnLogin <| Login.view model.login
                else
                    div [ class "signin" ] [ welcome, signout ]

        contributorsUI : Html Msg
        contributorsUI =
            Html.map ProfileThumbnail <|
                div [] (model.contributors |> List.map thumbnail)
    in
        div []
            [ header []
                [ label [] [ text "Nikeza" ]
                , model |> loginUI
                ]
            , input [ type_ "text", placeholder "name", onInput Search ] []
            , div [] [ contributorsUI ]
            , footer [ class "copyright" ]
                [ label [] [ text "(c)2017" ]
                , a [ href "" ] [ text "GitHub" ]
                ]
            ]


contributorTopicContentTypePage : Topic -> ContentType -> Contributor.Model -> Html Msg
contributorTopicContentTypePage topic contentType model =
    let
        profileId =
            model.profile.id

        links =
            runtime.topicLinks topic Video profileId
    in
        div []
            [ h2 [] [ text <| "All " ++ contentTypeToText contentType ]
            , table []
                [ tr []
                    [ td [] [ img [ src <| getUrl <| model.profile.imageUrl, width 100, height 100 ] [] ]
                    , td [] [ h2 [] [ text <| getTopic topic ] ]
                    , td [] [ div [] <| List.map (\link -> a [ href <| getUrl link.url ] [ text <| getTitle link.title, br [] [] ]) links ]
                    ]
                ]
            ]


contributorTopicPage : Contributor.Model -> Html Msg
contributorTopicPage model =
    let
        profileId =
            model.profile.id
    in
        case List.head model.topics of
            Just topic ->
                div []
                    [ table []
                        [ tr []
                            [ table []
                                [ tr []
                                    [ td [] [ img [ src <| getUrl <| model.profile.imageUrl, width 100, height 100 ] [] ]
                                    , table []
                                        [ tr [] [ h2 [] [ text <| getTopic topic ] ]
                                        , tr [] [ td [] [ b [] [ text "Videos" ] ] ]
                                        , div [] <| contentWithTopicUI profileId Video topic (runtime.topicLinks topic Video profileId)
                                        , tr [] [ td [] [ b [] [ text "Podcasts" ] ] ]
                                        , div [] <| contentWithTopicUI profileId Podcast topic (runtime.topicLinks topic Podcast profileId)
                                        , tr [] [ td [] [ b [] [ text "Articles" ] ] ]
                                        , div [] <| contentWithTopicUI profileId Article topic (runtime.topicLinks topic Article profileId)
                                        , tr [] [ td [] [ b [] [ text "Answers" ] ] ]
                                        , div [] <| contentWithTopicUI profileId Answer topic (runtime.topicLinks topic Answer profileId)
                                        ]
                                    ]
                                , tr [] [ td [] [ text <| getName model.profile.name ] ]
                                , tr [] [ td [] [ p [] [ text model.profile.bio ] ] ]
                                ]
                            ]
                        ]
                    ]

            Nothing ->
                notFoundPage


connectionUI : Connection -> Html Msg
connectionUI connection =
    tr []
        [ td [] [ text connection.platform ]
        , td [] [ i [] [ text connection.username ] ]
        , td [] [ button [ onClick <| Remove connection ] [ text "Disconnect" ] ]
        ]


content : Portal -> Html Msg
content portal =
    let
        contributor =
            portal.contributor

        connectionsTable =
            table [] [ div [] (contributor.profile.connections |> List.map connectionUI) ]
    in
        case portal.requested of
            Domain.ViewConnections ->
                table []
                    [ tr []
                        [ th [] [ h3 [] [ text "Connections" ] ] ]
                    , tr []
                        [ td [] [ Html.map NewConnection <| AddConnection.view portal.newConnection ] ]
                    , tr []
                        [ td [] [ connectionsTable ] ]
                    ]

            Domain.ViewLinks ->
                div []
                    [ Html.map ContributorLinksAction <| ContributorLinks.view contributor
                    ]

            Domain.AddLink ->
                let
                    linkSummary =
                        portal |> getLinkSummary

                    newLinkEditor =
                        Html.map NewLink (NewLinks.view (linkSummary))

                    addLink l =
                        div []
                            [ label [] [ text <| (l.contentType |> contentTypeToText |> dropRight 1) ++ ": " ]
                            , a [ href <| getUrl l.url ] [ text <| getTitle l.title ]
                            ]

                    update =
                        if linkSummary.canAdd then
                            div [] (linkSummary.added |> List.map addLink)
                        else
                            div [] []
                in
                    table []
                        [ tr []
                            [ th [] [ h3 [] [ text "Add Link" ] ] ]
                        , tr []
                            [ td [] [ newLinkEditor ] ]
                        , tr []
                            [ td [] [ update ] ]
                        ]


getLinkSummary : Portal -> NewLinks
getLinkSummary portal =
    portal.newLinks


dashboardPage : Model -> Html Msg
dashboardPage model =
    let
        contributor =
            model.portal.contributor

        linkSummary =
            portal |> getLinkSummary

        header =
            [ h2 [] [ text <| "Welcome " ++ getName model.portal.contributor.profile.name ] ]

        portal =
            model.portal
    in
        div []
            [ table []
                [ tr []
                    [ td []
                        [ table []
                            [ tr [] [ th [] header ]
                            , tr []
                                [ td [] [ img [ src <| getUrl <| contributor.profile.imageUrl, width 100, height 100 ] [] ]
                                , td []
                                    [ div []
                                        [ button [ onClick ViewConnections ] [ text "Connections" ]
                                        , br [] []
                                        , button [ onClick ViewLinks ] [ text "Links" ]
                                        , br [] []
                                        , button [ onClick AddNewLink ] [ text "Link" ]
                                        , br [] []
                                        , button [ onClick ViewLinks ] [ text "Subscribers" ]
                                        , br [] []
                                        , button [ onClick ViewLinks ] [ text "Subscriptions" ]
                                        ]
                                    ]
                                ]
                            , tr [] [ td [] [ p [] [ text contributor.profile.bio ] ] ]
                            ]
                        ]
                    , td [] [ content portal ]
                    ]
                ]
            ]


notFoundPage : Html Msg
notFoundPage =
    div [] [ text "Page not found" ]


linksUI : List Link -> List (Html Msg)
linksUI links =
    links
        |> List.take 5
        |> List.map (\link -> a [ href <| getUrl link.url ] [ text <| getTitle link.title, br [] [] ])


contentWithTopicUI : Id -> ContentType -> Topic -> List Link -> List (Html Msg)
contentWithTopicUI profileId contentType topic links =
    List.append (linksUI links) [ a [ href <| getUrl <| moreContributorContentOnTopicUrl profileId contentType topic ] [ text <| "all", br [] [] ] ]



-- NAVIGATION


type alias RoutePath =
    List String


tokenizeUrl : String -> RoutePath
tokenizeUrl urlHash =
    urlHash |> String.split "/" |> List.drop 1


navigate : Msg -> Model -> Location -> ( Model, Cmd Msg )
navigate msg model location =
    case tokenizeUrl location.hash of
        [ "contributor", id ] ->
            case runtime.contributor <| Id id of
                Just c ->
                    ( { model | selectedContributor = c, currentRoute = location }, Cmd.none )

                Nothing ->
                    ( { model | currentRoute = location }, Cmd.none )

        [ "contributor", id, topic ] ->
            case runtime.contributor <| Id id of
                Just contributor ->
                    let
                        topicContributor =
                            { contributor | topics = [ Topic topic ] }
                    in
                        ( { model | selectedContributor = topicContributor, currentRoute = location }, Cmd.none )

                Nothing ->
                    ( { model | currentRoute = location }, Cmd.none )

        _ ->
            ( { model | currentRoute = location }, Cmd.none )
