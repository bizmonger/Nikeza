module Home exposing (..)

import Settings exposing (runtime)
import Domain.Core as Domain exposing (..)
import Domain.ContributorPortal as ContributorPortal exposing (..)
import Domain.Contributor as Contributor exposing (..)
import Controls.Login as Login exposing (..)
import Controls.ProfileThumbnail as ProfileThumbnail exposing (..)
import Controls.AddConnection as AddConnection exposing (..)
import Controls.NewLinks as NewLinks exposing (..)
import Controls.ContributorLinks as ContributorLinks exposing (..)
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
    , portal : ContributorPortal.Model
    , contributors : List Profile
    , selectedContributor : Contributor.Model
    }


init : Navigation.Location -> ( Model, Cmd Msg )
init location =
    let
        contributor =
            case tokenizeUrl location.hash of
                [ "contributor", id ] ->
                    case runtime.contributor <| Id id of
                        Just p ->
                            getContributor p

                        Nothing ->
                            Contributor.init

                _ ->
                    Contributor.init
    in
        ( { currentRoute = location
          , login = Login.model
          , portal = ContributorPortal.init
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
                ( { model | portal = { pendingPortal | requested = Domain.Connections } }, Cmd.none )

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
                ( { model | portal = { pendingPortal | requested = Domain.Links } }, Cmd.none )

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
            { contributor | profile = updatedProfile, newConnection = initConnection }

        portal =
            { contributor = updatedContributor, requested = Links }

        newState =
            { model | portal = portal }
    in
        ( newState, Cmd.none )


onNewLink : NewLinks.Msg -> Model -> ( Model, Cmd Msg )
onNewLink subMsg model =
    let
        contributor =
            model.portal.contributor

        newState =
            NewLinks.update subMsg (contributor.newLinks)

        updatedContributor =
            { contributor | newLinks = newState }

        portal =
            { contributor = updatedContributor, requested = Links }
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
                ( model, Cmd.none )



--( { model | contributor = { contributor | newLinks = { newState | canAdd = True, added = v.current.base :: v.added } } }, Cmd.none )


onNewConnection : AddConnection.Msg -> Model -> ( Model, Cmd Msg )
onNewConnection subMsg model =
    let
        contributor =
            model.portal.contributor

        connection =
            AddConnection.update subMsg contributor.newConnection

        updatedContributor =
            { contributor | newConnection = connection }

        pendingPortal =
            model.portal

        updatedPortal =
            { pendingPortal | contributor = updatedContributor }

        pendingProfile =
            updatedContributor.profile

        portal =
            { updatedPortal | contributor = { updatedContributor | profile = pendingProfile } }
    in
        case subMsg of
            AddConnection.InputUsername _ ->
                ( { model | portal = portal }, Cmd.none )

            AddConnection.InputPlatform _ ->
                ( { model | portal = portal }, Cmd.none )

            AddConnection.Submit _ ->
                let
                    updatedProfile =
                        { pendingProfile | connections = connection :: pendingProfile.connections }
                in
                    ( { model
                        | portal =
                            { portal
                                | contributor = { updatedContributor | profile = updatedProfile }
                            }
                      }
                    , Cmd.none
                    )


matchContributors : Model -> String -> ( Model, Cmd Msg )
matchContributors model matchValue =
    let
        onName profile =
            profile.name
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

                    ( contributor, latest ) =
                        ( pendingPortal.contributor, runtime.tryLogin login )

                    newState =
                        case runtime.contributor <| runtime.usernameToId login.username of
                            Just p ->
                                let
                                    updatedProfile =
                                        { p | connections = p.id |> runtime.connections }
                                in
                                    { model
                                        | login = latest
                                        , portal = { pendingPortal | contributor = { contributor | profile = updatedProfile } }
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
                    Html.map ContributorLinksAction <| ContributorLinks.view model.selectedContributor

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
                Just _ ->
                    notFoundPage

                --contributorContentTypePage contentType model.contributor
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
            Html.map ProfileThumbnail (div [] (model.contributors |> List.map thumbnail))
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



-- contributorContentTypePage : String -> Contributor.Model -> Html Msg
-- contributorContentTypePage contentTypeText model =
--     let
--         topics =
--             model.profile.topics
--         posts =
--             case contentTypeText |> toContentType of
--                 Answer ->
--                     model.answers
--                 Article ->
--                     model.articles
--                 Podcast ->
--                     model.podcasts
--                 Video ->
--                     model.videos
--                 Unknown ->
--                     []
--                 All ->
--                     []
--     in
--         div []
--             [ h2 [] [ text <| "All " ++ contentTypeText ]
--             , table []
--                 [ tr []
--                     [ td [] [ img [ src <| getUrl <| model.profile.imageUrl, width 100, height 100 ] [] ]
--                     , td [] [ div [] (topics |> List.map toCheckbox) ]
--                     , td [] [ div [] <| List.map (\link -> a [ href <| getUrl link.url ] [ text <| getTitle link.title, br [] [] ]) posts ]
--                     ]
--                 ]
--             ]


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


content : ContributorPortal.Model -> Html Msg
content portal =
    let
        contributor =
            portal.contributor

        connectionsTable =
            table [] [ div [] (contributor.profile.connections |> List.map connectionUI) ]
    in
        case portal.requested of
            Domain.Connections ->
                table []
                    [ tr []
                        [ th [] [ h3 [] [ text "Connections" ] ] ]
                    , tr []
                        [ td [] [ Html.map NewConnection <| AddConnection.view contributor.newConnection ] ]
                    , tr []
                        [ td [] [ connectionsTable ] ]
                    ]

            Domain.AddLink ->
                let
                    linkSummary =
                        getLinkSummary contributor

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
                    -- [ Html.map NewLink (NewLinks.view (linkSummary))
                    -- , update
                    -- ]
                    div [] [ newLinkEditor, update ]

            Domain.Links ->
                label [] [ text "Links..." ]


getLinkSummary : Contributor.Model -> NewLinks.Model
getLinkSummary contributor =
    contributor.newLinks


dashboardPage : Model -> Html Msg
dashboardPage model =
    let
        contributor =
            model.portal.contributor

        linkSummary =
            contributor |> getLinkSummary

        header =
            [ h2 [] [ text <| "Welcome " ++ getName model.portal.contributor.profile.name ] ]

        portal =
            model.portal
    in
        div []
            [ table []
                [ tr [] [ th [] header ]
                , td [] [ img [ src <| getUrl <| contributor.profile.imageUrl, width 100, height 100 ] [] ]
                , td [] [ content model.portal ]
                ]
            , button [ onClick ViewConnections ] [ text "Connections" ]
            , br [] []
            , button [ onClick AddNewLink ] [ text "Link" ]
            , br [] []
            , button [ onClick ViewLinks ] [ text "Links" ]
            , br [] []
            ]



--, table []
--[
--tr []
--  [ th [] [ h3 [] [ text "Profile" ] ] ]
--   tr []
--     [ td [] [ td [] [ Html.map ContributorLinksAction <| ContributorLinks.view model.contributor ] ] ]
-- , tr []
--     [ th [] [ h3 [] [ text "Connections" ] ] ]
-- , tr []
--     [ td [] [ Html.map NewConnection <| AddConnection.view model.contributor.newConnection ] ]
-- , tr []
--     [ td [] [ connectionsTable ] ]
-- , tr []
--     [ th [] [ h3 [] [ text "Add Link" ] ] ]
-- , tr []
--     [ Html.map NewLink (NewLinks.view linkSummary) ]
-- , tr [] [ update ]
--]
--]


notFoundPage : Html Msg
notFoundPage =
    div [] [ text "Page not found" ]



-- toCheckbox : Topic -> Html Msg
-- toCheckbox topic =
--     div []
--         [ input [ type_ "checkbox", checked True, onCheck (\b -> ContributorLinks.Toggle ( topic, b )) ] []
--         , label [] [ text <| getTopic topic ]
--         ]
-- TODO: Move LinksUI to some UI module that needs to be created first.


linksUI : List Link -> List (Html Msg)
linksUI links =
    links
        |> List.take 5
        |> List.map (\link -> a [ href <| getUrl link.url ] [ text <| getTitle link.title, br [] [] ])



-- contentUI : Id -> ContentType -> List Link -> List (Html Msg)
-- contentUI profileId contentType links =
--     List.append (linksUI links) [ a [ href <| getUrl <| moreContributorContentUrl profileId contentType ] [ text <| "all", br [] [] ] ]


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
                Just profile ->
                    ( { model | selectedContributor = getContributor profile, currentRoute = location }, Cmd.none )

                Nothing ->
                    ( { model | currentRoute = location }, Cmd.none )

        [ "contributor", id, topic ] ->
            case runtime.contributor <| Id id of
                Just profile ->
                    let
                        contributor =
                            getContributor profile

                        topicContributor =
                            { contributor | topics = [ Topic topic ] }
                    in
                        ( { model | selectedContributor = topicContributor, currentRoute = location }, Cmd.none )

                Nothing ->
                    ( { model | currentRoute = location }, Cmd.none )

        _ ->
            ( { model | currentRoute = location }, Cmd.none )
