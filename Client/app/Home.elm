module Home exposing (..)

import Settings exposing (runtime)
import Domain.Core as Domain exposing (..)
import Domain.ContentProvider as ContentProvider exposing (..)
import Controls.Login as Login exposing (..)
import Controls.ProfileThumbnail as ProfileThumbnail exposing (..)
import Controls.AddSource as AddSource exposing (..)
import Controls.NewLinks as NewLinks exposing (..)
import Controls.ContentProviderLinks as ContentProviderLinks exposing (..)
import Controls.ContentProviderContentTypeLinks as ContentProviderContentTypeLinks exposing (..)
import Controls.ContentProviderTopicContentTypeLinks as ContentProviderTopicContentTypeLinks exposing (..)
import Controls.Register as Registration exposing (..)
import Controls.EditProfile as EditProfile exposing (..)
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
    , registration : Registration.Model
    , portal : Portal
    , contentProviders : List ContentProvider
    , selectedContentProvider : ContentProvider
    }


init : Navigation.Location -> ( Model, Cmd Msg )
init location =
    let
        contentProvider =
            case tokenizeUrl location.hash of
                [ "contentProvider", id ] ->
                    case runtime.contentProvider <| Id id of
                        Just contentProvider ->
                            contentProvider

                        Nothing ->
                            initContentProvider

                _ ->
                    initContentProvider
    in
        ( { currentRoute = location
          , login = Login.init
          , registration = Registration.model
          , portal = initPortal
          , contentProviders = runtime.contentProviders
          , selectedContentProvider = contentProvider
          }
        , Cmd.none
        )



-- UPDATE


type Msg
    = UrlChange Navigation.Location
    | OnLogin Login.Msg
    | ProfileThumbnail ProfileThumbnail.Msg
    | SourceAdded AddSource.Msg
    | ViewSources
    | AddNewLink
    | ViewLinks
    | EditProfile
    | ViewSubscriptions
    | ViewFollowers
    | ViewProviders
    | NewLink NewLinks.Msg
    | ContentProviderLinksAction ContentProviderLinks.Msg
    | PortalLinksAction ContentProviderLinks.Msg
    | EditProfileAction EditProfile.Msg
    | ContentProviderContentTypeLinksAction ContentProviderContentTypeLinks.Msg
    | ContentProviderTopicContentTypeLinksAction ContentProviderTopicContentTypeLinks.Msg
    | Search String
    | Register
    | OnRegistration Registration.Msg


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    let
        portal =
            model.portal
    in
        case msg of
            UrlChange location ->
                location |> navigate msg model

            Register ->
                ( model, Navigation.load <| "/#/register" )

            OnRegistration subMsg ->
                onRegistration subMsg model

            OnLogin subMsg ->
                onLogin subMsg model

            Search "" ->
                ( { model | contentProviders = runtime.contentProviders }, Cmd.none )

            Search text ->
                text |> matchContentProviders model

            ProfileThumbnail subMsg ->
                ( model, Cmd.none )

            ViewSources ->
                ( { model | portal = { portal | requested = Domain.ViewSources } }, Cmd.none )

            AddNewLink ->
                ( { model | portal = { portal | requested = Domain.AddLink } }, Cmd.none )

            ViewLinks ->
                ( { model | portal = { portal | requested = Domain.ViewLinks } }, Cmd.none )

            EditProfile ->
                ( { model | portal = { portal | requested = Domain.EditProfile } }, Cmd.none )

            ViewSubscriptions ->
                ( { model | portal = { portal | requested = Domain.ViewSubscriptions } }, Cmd.none )

            ViewFollowers ->
                ( { model | portal = { portal | requested = Domain.ViewFollowers } }, Cmd.none )

            ViewProviders ->
                ( { model | portal = { portal | requested = Domain.ViewProviders } }, Cmd.none )

            SourceAdded subMsg ->
                onAddedSource subMsg model

            NewLink subMsg ->
                onNewLink subMsg model

            EditProfileAction subMsg ->
                onEditProfile subMsg model

            PortalLinksAction subMsg ->
                onPortalLinksAction subMsg model

            ContentProviderLinksAction subMsg ->
                onUpdateContentProviderLinks subMsg model FromOther

            ContentProviderContentTypeLinksAction subMsg ->
                case subMsg of
                    ContentProviderContentTypeLinks.Toggle _ ->
                        let
                            contentProvider =
                                if model.portal.requested == Domain.ViewLinks then
                                    ContentProviderContentTypeLinks.update subMsg model.portal.contentProvider
                                else
                                    ContentProviderContentTypeLinks.update subMsg model.selectedContentProvider
                        in
                            if model.portal.requested == Domain.ViewLinks then
                                ( { model | portal = { portal | contentProvider = contentProvider } }, Cmd.none )
                            else
                                ( { model | selectedContentProvider = contentProvider }, Cmd.none )

            ContentProviderTopicContentTypeLinksAction subMsg ->
                ( model, Cmd.none )


onUpdateContentProviderLinks : ContentProviderLinks.Msg -> Model -> Linksfrom -> ( Model, Cmd Msg )
onUpdateContentProviderLinks subMsg model linksfrom =
    case subMsg of
        ContentProviderLinks.Toggle _ ->
            let
                contentProvider =
                    case linksfrom of
                        FromPortal ->
                            ContentProviderLinks.update subMsg model.portal.contentProvider

                        FromOther ->
                            ContentProviderLinks.update subMsg model.selectedContentProvider
            in
                ( { model | selectedContentProvider = contentProvider }, Cmd.none )


onPortalLinksAction : ContentProviderLinks.Msg -> Model -> ( Model, Cmd Msg )
onPortalLinksAction subMsg model =
    case subMsg of
        ContentProviderLinks.Toggle _ ->
            let
                contentProvider =
                    ContentProviderLinks.update subMsg model.portal.contentProvider

                pendingPortal =
                    model.portal
            in
                ( { model | portal = { pendingPortal | contentProvider = contentProvider } }, Cmd.none )


onEditProfile : EditProfile.Msg -> Model -> ( Model, Cmd Msg )
onEditProfile subMsg model =
    let
        updatedProfile =
            EditProfile.update subMsg model.portal.contentProvider.profile

        portal =
            model.portal

        contentProvider =
            model.portal.contentProvider

        newState =
            { model | portal = { portal | contentProvider = { contentProvider | profile = updatedProfile } } }
    in
        case subMsg of
            EditProfile.FirstNameInput _ ->
                ( newState, Cmd.none )

            EditProfile.LastNameInput _ ->
                ( newState, Cmd.none )

            EditProfile.EmailInput _ ->
                ( newState, Cmd.none )

            EditProfile.BioInput _ ->
                ( newState, Cmd.none )

            EditProfile.Save v ->
                ( { model
                    | portal =
                        { portal
                            | contentProvider = { contentProvider | profile = v }
                            , sourcesNavigation = True
                            , linksNavigation = not <| contentProvider.links == initLinks
                            , requested = Domain.ViewSources
                        }
                  }
                , Cmd.none
                )


onRegistration : Registration.Msg -> Model -> ( Model, Cmd Msg )
onRegistration subMsg model =
    let
        form =
            Registration.update subMsg model.registration
    in
        case subMsg of
            Registration.FirstNameInput _ ->
                ( { model | registration = form }, Cmd.none )

            Registration.LastNameInput _ ->
                ( { model | registration = form }, Cmd.none )

            Registration.EmailInput _ ->
                ( { model | registration = form }, Cmd.none )

            Registration.PasswordInput _ ->
                ( { model | registration = form }, Cmd.none )

            Registration.ConfirmInput _ ->
                ( { model | registration = form }, Cmd.none )

            Registration.Submit _ ->
                case form |> runtime.tryRegister of
                    Ok newUser ->
                        let
                            newState =
                                { model
                                    | registration = form
                                    , portal =
                                        { initPortal
                                            | contentProvider = newUser
                                            , requested = Domain.EditProfile
                                            , linksNavigation = False
                                            , sourcesNavigation = False
                                        }
                                }
                        in
                            ( newState, Navigation.load <| "/#/" ++ getId newUser.profile.id ++ "/portal" )

                    Err v ->
                        ( model, Cmd.none )


onRemove : Model -> Source -> ( Model, Cmd Msg )
onRemove model sources =
    let
        contentProvider =
            model.portal.contentProvider

        profile =
            contentProvider.profile

        sourcesLeft =
            profile.sources |> List.filter (\c -> c /= sources)

        updatedProfile =
            { profile | sources = sourcesLeft }

        updatedContentProvider =
            { contentProvider | profile = updatedProfile }

        pendingPortal =
            model.portal

        portal =
            { pendingPortal | contentProvider = updatedContentProvider, newSource = initSource }

        newState =
            { model | portal = portal }
    in
        ( newState, Cmd.none )


refreshLinks : ContentProvider -> List Link -> Links
refreshLinks contentProvider addedLinks =
    let
        links =
            contentProvider.links

        articles =
            List.append links.articles (addedLinks |> List.filter (\l -> l.contentType == Article))

        videos =
            List.append links.videos (addedLinks |> List.filter (\l -> l.contentType == Video))

        podcasts =
            List.append links.podcasts (addedLinks |> List.filter (\l -> l.contentType == Podcast))

        answers =
            List.append links.answers (addedLinks |> List.filter (\l -> l.contentType == Answer))
    in
        { articles = articles
        , videos = videos
        , podcasts = podcasts
        , answers = answers
        }


onNewLink : NewLinks.Msg -> Model -> ( Model, Cmd Msg )
onNewLink subMsg model =
    let
        ( pendingPortal, contentProvider ) =
            ( model.portal, model.portal.contentProvider )

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
                        { portal
                            | newLinks = updatedLinks
                            , linksNavigation = True
                            , contentProvider = { contentProvider | links = refreshLinks contentProvider updatedLinks.added }
                        }
                in
                    ( { model | portal = updatedPortal }, Cmd.none )


onAddedSource : AddSource.Msg -> Model -> ( Model, Cmd Msg )
onAddedSource subMsg model =
    let
        ( pendingPortal, contentProvider, updatedProfile ) =
            ( model.portal, model.portal.contentProvider, model.portal.contentProvider.profile )

        addSourceModel =
            AddSource.update subMsg { source = pendingPortal.newSource, sources = contentProvider.profile.sources }

        updatedContentProvider =
            { contentProvider | profile = { updatedProfile | sources = addSourceModel.sources } }

        portal =
            { pendingPortal | newSource = addSourceModel.source, contentProvider = updatedContentProvider }
    in
        case subMsg of
            AddSource.InputUsername _ ->
                ( { model | portal = portal }, Cmd.none )

            AddSource.InputPlatform _ ->
                ( { model | portal = portal }, Cmd.none )

            AddSource.Add _ ->
                ( { model
                    | portal =
                        { portal
                            | linksNavigation = linksExist contentProvider.links
                            , addLinkNavigation = True
                            , sourcesNavigation = True
                        }
                  }
                , Cmd.none
                )

            AddSource.Remove _ ->
                ( { model
                    | portal =
                        { portal
                            | linksNavigation = linksExist portal.contentProvider.links
                            , sourcesNavigation = True
                            , addLinkNavigation = True
                        }
                  }
                , Cmd.none
                )


matchContentProviders : Model -> String -> ( Model, Cmd Msg )
matchContentProviders model matchValue =
    let
        isMatch name =
            name |> toLower |> contains (matchValue |> toLower)

        onFirstName contentProvider =
            contentProvider.profile.firstName |> getName |> isMatch

        onLastName contentProvider =
            contentProvider.profile.lastName |> getName |> isMatch

        onName contentProvider =
            onFirstName contentProvider || onLastName contentProvider

        filtered =
            runtime.contentProviders |> List.filter onName
    in
        ( { model | contentProviders = filtered }, Cmd.none )


onLogin : Login.Msg -> Model -> ( Model, Cmd Msg )
onLogin subMsg model =
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

                    contentProviderResult =
                        runtime.contentProvider <| runtime.usernameToId latest.email

                    newState =
                        case contentProviderResult of
                            Just contentProvider ->
                                { model
                                    | login = latest
                                    , portal =
                                        { pendingPortal
                                            | contentProvider = contentProvider
                                            , requested = Domain.ViewLinks
                                            , linksNavigation = linksExist contentProvider.links
                                            , sourcesNavigation = not <| List.isEmpty contentProvider.profile.sources
                                        }
                                }

                            Nothing ->
                                { model | login = latest }
                in
                    if newState.login.loggedIn then
                        ( newState, Navigation.load <| "/#/" ++ getId newState.portal.contentProvider.profile.id ++ "/portal" )
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

        [ "register" ] ->
            let
                content =
                    registerPage model
            in
                model |> renderPage content

        [ "contentProvider", id ] ->
            case runtime.contentProvider <| Id id of
                Just _ ->
                    let
                        content =
                            renderProfileBase model.selectedContentProvider <| Html.map ContentProviderLinksAction <| ContentProviderLinks.view FromOther model.selectedContentProvider
                    in
                        model |> renderPage content

                Nothing ->
                    notFoundPage

        [ "contentProvider", id, topic ] ->
            case runtime.contentProvider <| Id id of
                Just _ ->
                    let
                        content =
                            contentProviderTopicPage FromOther model.selectedContentProvider
                    in
                        model |> renderPage content

                Nothing ->
                    notFoundPage

        [ "contentProvider", id, "all", contentType ] ->
            case runtime.contentProvider <| Id id of
                Just _ ->
                    let
                        ( view, contentProvider ) =
                            ( ContentProviderContentTypeLinks.view, model.selectedContentProvider )

                        contentToEmbed =
                            Html.map ContentProviderContentTypeLinksAction <| view contentProvider <| toContentType contentType

                        content =
                            renderProfileBase model.selectedContentProvider <| contentToEmbed
                    in
                        model |> renderPage content

                Nothing ->
                    notFoundPage

        [ "contentProvider", id, topicName, "all", contentType ] ->
            case runtime.contentProvider <| Id id of
                Just _ ->
                    let
                        topic =
                            Topic topicName False

                        contentToEmbed =
                            Html.map ContentProviderTopicContentTypeLinksAction <| ContentProviderTopicContentTypeLinks.view model.selectedContentProvider topic <| toContentType contentType

                        content =
                            renderProfileBase model.selectedContentProvider <| contentToEmbed
                    in
                        model |> renderPage content

                Nothing ->
                    notFoundPage

        [ id, "portal", "all", contentType ] ->
            case runtime.contentProvider <| Id id of
                Just _ ->
                    let
                        linksContent =
                            Html.map ContentProviderContentTypeLinksAction <| ContentProviderContentTypeLinks.view model.portal.contentProvider <| toContentType contentType

                        contentToEmbed =
                            linksContent |> applyToPortal id model contentType

                        mainContent =
                            model.portal |> content (Just contentToEmbed)
                    in
                        model |> renderPage mainContent

                Nothing ->
                    notFoundPage

        [ id, "portal" ] ->
            let
                ( portal, contentType ) =
                    ( model.portal, "all" )

                mainContent =
                    portal
                        |> content Nothing
                        |> applyToPortal id model contentType
            in
                model |> renderPage mainContent

        _ ->
            notFoundPage


renderProfileBase : ContentProvider -> Html Msg -> Html Msg
renderProfileBase contentProvider linksContent =
    table []
        [ tr []
            [ table []
                [ tr [ class "bio" ] [ td [] [ img [ class "profile", src <| getUrl <| contentProvider.profile.imageUrl ] [] ] ]
                , tr [ class "bio" ] [ td [] [ text <| getName contentProvider.profile.firstName ++ " " ++ getName contentProvider.profile.lastName ] ]
                , tr [ class "bio" ] [ td [] [ p [] [ text contentProvider.profile.bio ] ] ]
                ]
            , td [] [ linksContent ]
            ]
        ]


applyToPortal : String -> Model -> String -> Html Msg -> Html Msg
applyToPortal profileId model contentType linksContent =
    let
        portal =
            model.portal
    in
        if portal.contentProvider == initContentProvider then
            case runtime.contentProvider <| Id profileId of
                Just contentProvider ->
                    portal |> render contentProvider contentType linksContent

                Nothing ->
                    notFoundPage
        else
            portal |> render portal.contentProvider contentType linksContent


render : ContentProvider -> String -> Html Msg -> Portal -> Html Msg
render contentProvider contentType linksContent portal =
    table []
        [ tr []
            [ td []
                [ table []
                    [ tr [ class "bio" ] [ td [] [ img [ class "profile", src <| getUrl <| contentProvider.profile.imageUrl ] [] ] ]
                    , tr [] [ td [] <| renderNavigation portal ]
                    ]
                ]
            , td [] [ linksContent ]
            ]
        ]


headerContent : Model -> Html Msg
headerContent model =
    let
        loginUI : Model -> Html Msg
        loginUI model =
            let
                ( loggedIn, welcome, signout ) =
                    ( model.login.loggedIn
                    , p [] [ text <| "Welcome " ++ model.login.email ++ "!" ]
                    , a [ href "" ] [ label [] [ text "Signout" ] ]
                    )
            in
                if (not loggedIn) then
                    Html.map OnLogin <| Login.view model.login
                else
                    div [ class "signin" ] [ welcome, signout ]
    in
        div []
            [ header [ class "header" ]
                [ img [ src "assets/Nikeza_thin_2.png", width 190, height 38 ] []
                , br [] []
                , label [] [ i [] [ text "Linking Your Expertise" ] ]
                , model |> loginUI
                ]
            , input [ class "search", type_ "text", placeholder "name", onInput Search ] []
            ]


footerContent : Html Msg
footerContent =
    footer [ class "copyright" ]
        [ label [] [ text "2017" ]
        , a [ href "" ] [ text "GitHub" ]
        ]


contentProvidersUI : Model -> Html Msg
contentProvidersUI model =
    Html.map ProfileThumbnail <|
        div [] (model.contentProviders |> List.map thumbnail)


homePage : Model -> Html Msg
homePage model =
    let
        mainContent =
            table []
                [ tr []
                    [ td [] [ div [] [ contentProvidersUI model ] ]
                    , td []
                        [ table []
                            [ tr []
                                [ td [] [ button [ class "join", onClick Register ] [ text "Join!" ] ]
                                , td []
                                    [ ul [ class "featuresList" ]
                                        [ li [ class "joinReasons" ] [ text "Import links to your articles, videos, and answers" ]
                                        , li [ class "joinReasons" ] [ text "Set your featured links for viewers to see" ]
                                        , li [ class "joinReasons" ] [ text "Subscribe to new links from your favorite thought leaders" ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
    in
        model |> renderPage mainContent


renderPage : Html Msg -> Model -> Html Msg
renderPage content model =
    div []
        [ headerContent model
        , content
        , footerContent
        ]


registerPage : Model -> Html Msg
registerPage model =
    div []
        [ h3 [] [ text "Join" ]
        , Html.map OnRegistration <| Registration.view model.registration
        ]


contentProviderTopicPage : Linksfrom -> ContentProvider.Model -> Html Msg
contentProviderTopicPage linksfrom model =
    let
        profileId =
            model.profile.id

        contentTable topic =
            table []
                [ tr [] [ h2 [] [ text <| getTopic topic ] ]
                , tr []
                    [ td [] [ b [] [ text "Answers" ] ]
                    , td [] [ b [] [ text "Articles" ] ]
                    ]
                , tr []
                    [ td [] [ div [] <| contentWithTopicUI linksfrom profileId Answer topic (runtime.topicLinks topic Answer profileId) ]
                    , td [] [ div [] <| contentWithTopicUI linksfrom profileId Article topic (runtime.topicLinks topic Article profileId) ]
                    ]
                , tr []
                    [ td [] [ b [] [ text "Podcasts" ] ]
                    , td [] [ b [] [ text "Videos" ] ]
                    ]
                , tr []
                    [ td [] [ div [] <| contentWithTopicUI linksfrom profileId Podcast topic (runtime.topicLinks topic Podcast profileId) ]
                    , td [] [ div [] <| contentWithTopicUI linksfrom profileId Video topic (runtime.topicLinks topic Video profileId) ]
                    ]
                ]
    in
        case List.head model.topics of
            Just topic ->
                table []
                    [ tr []
                        [ td []
                            [ table []
                                [ tr [ class "bio" ] [ td [] [ img [ class "profile", src <| getUrl <| model.profile.imageUrl ] [] ] ]
                                , tr [ class "bio" ] [ td [] [ text <| getName model.profile.firstName ++ " " ++ getName model.profile.lastName ] ]
                                , tr [ class "bio" ] [ td [] [ p [] [ text model.profile.bio ] ] ]
                                ]
                            ]
                        , td [] [ contentTable topic ]
                        ]
                    ]

            Nothing ->
                notFoundPage


content : Maybe (Html Msg) -> Portal -> Html Msg
content contentToEmbed portal =
    let
        contentProvider =
            portal.contentProvider
    in
        case portal.requested of
            Domain.ViewSources ->
                div []
                    [ Html.map SourceAdded <|
                        AddSource.view
                            { source = portal.newSource
                            , sources = contentProvider.profile.sources
                            }
                    ]

            Domain.ViewLinks ->
                let
                    contentToDisplay =
                        case contentToEmbed of
                            Just v ->
                                v

                            Nothing ->
                                div [] [ Html.map PortalLinksAction <| ContentProviderLinks.view FromPortal contentProvider ]
                in
                    contentToDisplay

            Domain.EditProfile ->
                div [] [ Html.map EditProfileAction <| EditProfile.view contentProvider.profile ]

            Domain.AddLink ->
                let
                    linkSummary =
                        portal.newLinks

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
                        [ tr [] [ td [] [ newLinkEditor ] ]
                        , tr [] [ td [] [ update ] ]
                        ]

            Domain.ViewSubscriptions ->
                div [] [ label [] [ text "Following..." ] ]

            Domain.ViewFollowers ->
                div [] [ label [] [ text "Subscribers..." ] ]

            Domain.ViewProviders ->
                div [] [ label [] [ text "Providers..." ] ]


renderNavigation : Portal -> List (Html Msg)
renderNavigation portal =
    let
        links =
            portal.contentProvider.links

        totalLinks =
            (List.length links.answers)
                + (List.length links.articles)
                + (List.length links.videos)
                + (List.length links.podcasts)

        totalSubscriptions =
            0

        totalFollowers =
            0

        totalProviders =
            0

        profile =
            portal.contentProvider.profile

        sourcesText =
            "Sources " ++ "(" ++ (toString <| List.length profile.sources) ++ ")"

        linksText =
            "Links " ++ "(" ++ (toString totalLinks) ++ ")"

        followingText =
            "Following " ++ "(" ++ (toString totalSubscriptions) ++ ")"

        followersText =
            "Subscribers " ++ "(" ++ (toString totalFollowers) ++ ")"

        browseText =
            "Browse " ++ "(" ++ (toString totalProviders) ++ ")"

        ( linkText, profileText ) =
            ( "Link", "Profile" )

        allNavigation =
            case portal.requested of
                Domain.ViewSources ->
                    [ button [ class "navigationButton4", onClick ViewLinks ] [ text linksText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewSubscriptions ] [ text followingText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewFollowers ] [ text followersText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewProviders ] [ text browseText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick AddNewLink ] [ text linkText ]
                    , br [] []
                    , button [ class "selectedNavigationButton4", onClick ViewSources ] [ text sourcesText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick EditProfile ] [ text profileText ]
                    , br [] []
                    ]

                Domain.ViewLinks ->
                    [ button [ class "selectedNavigationButton4", onClick ViewLinks ] [ text linksText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewSubscriptions ] [ text followingText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewFollowers ] [ text followersText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewProviders ] [ text browseText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick AddNewLink ] [ text linkText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewSources ] [ text sourcesText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick EditProfile ] [ text profileText ]
                    , br [] []
                    ]

                Domain.AddLink ->
                    [ button [ class "navigationButton4", onClick ViewLinks ] [ text linksText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewSubscriptions ] [ text followingText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewFollowers ] [ text followersText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewProviders ] [ text browseText ]
                    , br [] []
                    , br [] []
                    , button [ class "selectedNavigationButton4", onClick AddNewLink ] [ text linkText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewSources ] [ text sourcesText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick EditProfile ] [ text profileText ]
                    , br [] []
                    ]

                Domain.EditProfile ->
                    [ button [ class "navigationButton4", onClick ViewLinks ] [ text linksText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewSubscriptions ] [ text followingText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewFollowers ] [ text followersText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewProviders ] [ text browseText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick AddNewLink ] [ text linkText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewSources ] [ text sourcesText ]
                    , br [] []
                    , button [ class "selectedNavigationButton4", onClick EditProfile ] [ text profileText ]
                    , br [] []
                    ]

                Domain.ViewSubscriptions ->
                    [ button [ class "navigationButton4", onClick ViewLinks ] [ text linksText ]
                    , br [] []
                    , button [ class "selectedNavigationButton4", onClick ViewSubscriptions ] [ text followingText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewFollowers ] [ text followersText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewProviders ] [ text browseText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick AddNewLink ] [ text linkText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewSources ] [ text sourcesText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick EditProfile ] [ text profileText ]
                    , br [] []
                    ]

                Domain.ViewFollowers ->
                    [ button [ class "navigationButton4", onClick ViewLinks ] [ text linksText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewSubscriptions ] [ text followingText ]
                    , br [] []
                    , button [ class "selectedNavigationButton4", onClick ViewFollowers ] [ text followersText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewProviders ] [ text browseText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick AddNewLink ] [ text linkText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewSources ] [ text sourcesText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick EditProfile ] [ text profileText ]
                    , br [] []
                    ]

                Domain.ViewProviders ->
                    [ button [ class "navigationButton4", onClick ViewLinks ] [ text linksText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewSubscriptions ] [ text followingText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewFollowers ] [ text followersText ]
                    , br [] []
                    , br [] []
                    , button [ class "selectedNavigationButton4", onClick ViewProviders ] [ text browseText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick AddNewLink ] [ text linkText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewSources ] [ text sourcesText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick EditProfile ] [ text profileText ]
                    , br [] []
                    ]

        sourcesButNoLinks =
            let
                noSelectedButton =
                    [ button [ class "navigationButton3", onClick ViewSources ] [ text sourcesText ]
                    , br [] []
                    , button [ class "navigationButton3", onClick AddNewLink ] [ text linkText ]
                    , br [] []
                    , button [ class "navigationButton3", onClick EditProfile ] [ text profileText ]
                    ]
            in
                case portal.requested of
                    Domain.ViewSources ->
                        [ button [ class "selectedNavigationButton3", onClick ViewSources ] [ text sourcesText ]
                        , br [] []
                        , button [ class "navigationButton3", onClick AddNewLink ] [ text linkText ]
                        , br [] []
                        , button [ class "navigationButton3", onClick EditProfile ] [ text profileText ]
                        ]

                    Domain.AddLink ->
                        [ button [ class "navigationButton3", onClick ViewSources ] [ text sourcesText ]
                        , br [] []
                        , button [ class "selectedNavigationButton3", onClick AddNewLink ] [ text linkText ]
                        , br [] []
                        , button [ class "navigationButton3", onClick EditProfile ] [ text profileText ]
                        ]

                    Domain.EditProfile ->
                        [ button [ class "navigationButton3", onClick ViewSources ] [ text sourcesText ]
                        , br [] []
                        , button [ class "navigationButton3", onClick AddNewLink ] [ text linkText ]
                        , br [] []
                        , button [ class "selectedNavigationButton3", onClick EditProfile ] [ text profileText ]
                        ]

                    Domain.ViewLinks ->
                        noSelectedButton

                    Domain.ViewSubscriptions ->
                        noSelectedButton

                    Domain.ViewFollowers ->
                        noSelectedButton

                    Domain.ViewProviders ->
                        noSelectedButton

        noSourcesNoLinks =
            case portal.requested of
                Domain.AddLink ->
                    [ button [ class "navigationButton3", onClick EditProfile ] [ text profileText ]
                    , br [] []
                    , button [ class "navigationButton3", onClick ViewSources, disabled True ] [ text sourcesText ]
                    , br [] []
                    , button [ class "selectedNavigationButton3", onClick AddNewLink, disabled True ] [ text linkText ]
                    ]

                Domain.EditProfile ->
                    [ button [ class "selectedNavigationButton3", onClick EditProfile ] [ text profileText ]
                    , br [] []
                    , button [ class "navigationButton3", onClick ViewSources, disabled True ] [ text sourcesText ]
                    , br [] []
                    , button [ class "navigationButton3", onClick AddNewLink, disabled True ] [ text linkText ]
                    ]

                _ ->
                    [ button [ class "navigationButton3", onClick EditProfile ] [ text profileText ]
                    , br [] []
                    , button [ class "navigationButton3", onClick ViewSources, disabled True ] [ text sourcesText ]
                    , br [] []
                    , button [ class "navigationButton3", onClick AddNewLink, disabled True ] [ text linkText ]
                    ]

        displayNavigation buttons =
            [ div [ class "navigationpane" ] buttons ]
    in
        if not portal.sourcesNavigation && not portal.linksNavigation then
            displayNavigation noSourcesNoLinks
        else if portal.sourcesNavigation && not portal.linksNavigation then
            displayNavigation sourcesButNoLinks
        else
            displayNavigation allNavigation


notFoundPage : Html Msg
notFoundPage =
    div [] [ text "Page not found" ]


linksUI : List Link -> List (Html Msg)
linksUI links =
    links
        |> List.take 5
        |> List.map (\link -> a [ href <| getUrl link.url ] [ text <| getTitle link.title, br [] [] ])


contentWithTopicUI : Linksfrom -> Id -> ContentType -> Topic -> List Link -> List (Html Msg)
contentWithTopicUI linksFrom profileId contentType topic links =
    List.append (linksUI links) [ a [ href <| getUrl <| allTopicContentUrl linksFrom profileId contentType topic ] [ text <| "all", br [] [] ] ]



-- NAVIGATION


type alias RoutePath =
    List String


tokenizeUrl : String -> RoutePath
tokenizeUrl urlHash =
    urlHash |> String.split "/" |> List.drop 1


navigate : Msg -> Model -> Location -> ( Model, Cmd Msg )
navigate msg model location =
    case tokenizeUrl location.hash of
        [ "contentProvider", id ] ->
            case runtime.contentProvider <| Id id of
                Just c ->
                    ( { model | selectedContentProvider = c, currentRoute = location }, Cmd.none )

                Nothing ->
                    ( { model | currentRoute = location }, Cmd.none )

        [ "contentProvider", id, "all", contentType ] ->
            case runtime.contentProvider <| Id id of
                Just c ->
                    ( { model | selectedContentProvider = c, currentRoute = location }, Cmd.none )

                Nothing ->
                    ( { model | currentRoute = location }, Cmd.none )

        [ "contentProvider", id, topic ] ->
            case runtime.contentProvider <| Id id of
                Just contentProvider ->
                    let
                        topicContentProvider =
                            { contentProvider | topics = [ Topic topic False ] }
                    in
                        ( { model | selectedContentProvider = topicContentProvider, currentRoute = location }, Cmd.none )

                Nothing ->
                    ( { model | currentRoute = location }, Cmd.none )

        [ id, "portal" ] ->
            case runtime.contentProvider <| Id id of
                Just c ->
                    let
                        portal =
                            model.portal

                        pendingPortal =
                            { portal
                                | contentProvider = c
                                , sourcesNavigation = c.profile.sources |> List.isEmpty
                                , addLinkNavigation = True
                                , linksNavigation = linksExist c.links
                                , requested = Domain.ViewLinks
                            }
                    in
                        ( { model | portal = pendingPortal, currentRoute = location }, Cmd.none )

                Nothing ->
                    ( { model | currentRoute = location }, Cmd.none )

        [ id, "portal", topic ] ->
            case runtime.contentProvider <| Id id of
                Just contentProvider ->
                    let
                        topicContentProvider =
                            { contentProvider | topics = [ Topic topic False ] }

                        portal =
                            model.portal

                        pendingPortal =
                            { portal | contentProvider = topicContentProvider }
                    in
                        ( { model | portal = pendingPortal, currentRoute = location }, Cmd.none )

                Nothing ->
                    ( { model | currentRoute = location }, Cmd.none )

        [ id, "portal", "all", contentType ] ->
            case runtime.contentProvider <| Id id of
                Just c ->
                    let
                        portal =
                            model.portal

                        pendingPortal =
                            { portal
                                | contentProvider = c
                                , sourcesNavigation = c.profile.sources |> List.isEmpty
                                , addLinkNavigation = True
                                , linksNavigation = linksExist c.links
                                , requested = Domain.ViewLinks
                            }
                    in
                        ( { model | portal = pendingPortal, currentRoute = location }, Cmd.none )

                Nothing ->
                    ( { model | currentRoute = location }, Cmd.none )

        _ ->
            ( { model | currentRoute = location }, Cmd.none )
