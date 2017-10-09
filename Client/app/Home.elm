module Home exposing (..)

import Settings exposing (runtime)
import Domain.Core as Domain exposing (..)
import Controls.Login as Login exposing (..)
import Controls.ProfileThumbnail as ProfileThumbnail exposing (..)
import Controls.RecentProviderLinks as RecentProviderLinks exposing (..)
import Controls.AddSource as AddSource exposing (..)
import Controls.NewLinks as NewLinks exposing (..)
import Controls.ProviderLinks as ProviderLinks exposing (..)
import Controls.ProviderContentTypeLinks as ProviderContentTypeLinks exposing (..)
import Controls.ProviderTopicContentTypeLinks as ProviderTopicContentTypeLinks exposing (..)
import Controls.Register as Registration exposing (..)
import Controls.EditProfile as EditProfile exposing (..)
import Services.Adapter exposing (..)
import Html exposing (..)
import Http
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onCheck, onInput)
import Navigation exposing (..)
import String exposing (..)


-- elm-live Home.elm --open --output=home.js --debug
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
    , login : Credentials
    , registration : Form
    , portal : Portal
    , providers : List Provider
    , selectedProvider : Provider
    }


init : Navigation.Location -> ( Model, Cmd Msg )
init location =
    ( { currentRoute = location
      , login = Login.init
      , registration = initForm
      , portal = initPortal
      , providers = []
      , selectedProvider = initProvider
      }
    , runtime.providers ProvidersResponse
    )



-- UPDATE


type Msg
    = UrlChange Navigation.Location
    | OnLogin Login.Msg
    | ProfileThumbnail ProfileThumbnail.Msg
    | RecentProviderLinks RecentProviderLinks.Msg
    | SourceAdded AddSource.Msg
    | ViewSources
    | AddNewLink
    | ViewLinks
    | EditProfile
    | ViewSubscriptions
    | ViewFollowers
    | ViewProviders
    | ViewRecent
    | NewLink NewLinks.Msg
    | ProviderLinksAction ProviderLinks.Msg
    | PortalLinksAction ProviderLinks.Msg
    | EditProfileAction EditProfile.Msg
    | ProviderContentTypeLinksAction ProviderContentTypeLinks.Msg
    | ProviderTopicContentTypeLinksAction ProviderTopicContentTypeLinks.Msg
    | ProvidersResponse (Result Http.Error (List JsonProvider))
    | NavigateToPortalResponse (Result Http.Error JsonProvider)
    | NavigateToPortalProviderTopicResponse (Result Http.Error JsonProvider)
    | NavigateToPortalProviderMemberResponse (Result Http.Error JsonProvider)
    | NavigateToPortalProviderMemberTopicResponse (Result Http.Error JsonProvider)
    | NavigateToProviderResponse (Result Http.Error JsonProvider)
    | NavigateToProviderTopicResponse (Result Http.Error JsonProvider)
    | Search String
    | Register
    | OnRegistration Registration.Msg
    | Subscription SubscriptionUpdate
    | NavigateBack


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    let
        portal =
            model.portal
    in
        case msg of
            UrlChange location ->
                location |> navigate msg model

            ProvidersResponse response ->
                case response of
                    Ok jsonProviders ->
                        let
                            providers =
                                jsonProviders |> List.map (\p -> p |> toProvider)
                        in
                            ( { model | providers = providers }, Cmd.none )

                    Err _ ->
                        ( model, Cmd.none )

            NavigateToPortalProviderTopicResponse response ->
                case response of
                    Ok jsonProvider ->
                        let
                            provider =
                                jsonProvider |> toProvider

                            portal =
                                model.portal

                            pendingPortal =
                                { portal
                                    | provider = jsonProvider |> toProvider
                                    , sourcesNavigation = provider.profile.sources |> List.isEmpty
                                    , addLinkNavigation = True
                                    , linksNavigation = portfolioExists provider.portfolio
                                    , requested = Domain.ViewRecent
                                }
                        in
                            ( { model | portal = pendingPortal }, Cmd.none )

                    Err _ ->
                        ( model, Cmd.none )

            NavigateToPortalProviderMemberTopicResponse response ->
                case response of
                    Ok jsonProvider ->
                        ( { model | selectedProvider = jsonProvider |> toProvider }, Cmd.none )

                    Err _ ->
                        ( model, Cmd.none )

            NavigateToPortalProviderMemberResponse response ->
                case response of
                    Ok jsonProvider ->
                        ( { model | selectedProvider = jsonProvider |> toProvider }, Cmd.none )

                    Err _ ->
                        ( model, Cmd.none )

            NavigateToProviderResponse response ->
                case response of
                    Ok jsonProvider ->
                        ( { model | selectedProvider = jsonProvider |> toProvider }, Cmd.none )

                    Err _ ->
                        ( model, Cmd.none )

            NavigateToProviderTopicResponse response ->
                case response of
                    Ok jsonProvider ->
                        ( { model | selectedProvider = jsonProvider |> toProvider }, Cmd.none )

                    Err _ ->
                        ( model, Cmd.none )

            NavigateToPortalResponse response ->
                case response of
                    Ok jsonProvider ->
                        let
                            ( portal, provider ) =
                                ( model.portal, jsonProvider |> toProvider )

                            pendingPortal =
                                { portal
                                    | provider = provider
                                    , sourcesNavigation = provider.profile.sources |> List.isEmpty
                                    , addLinkNavigation = True
                                    , linksNavigation = portfolioExists provider.portfolio
                                    , requested = Domain.ViewRecent
                                }
                        in
                            ( { model | portal = pendingPortal }, Cmd.none )

                    Err _ ->
                        ( model, Cmd.none )

            Register ->
                ( model, Navigation.load <| "/#/register" )

            OnRegistration subMsg ->
                onRegistration subMsg model

            OnLogin subMsg ->
                onLogin subMsg model

            Search "" ->
                ( model, runtime.providers ProvidersResponse )

            Search text ->
                text |> matchProviders model

            ProfileThumbnail subMsg ->
                ( model, Cmd.none )

            RecentProviderLinks subMsg ->
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

            ViewRecent ->
                ( { model | portal = { portal | requested = Domain.ViewRecent } }, Cmd.none )

            SourceAdded subMsg ->
                onAddedSource subMsg model

            NewLink subMsg ->
                onNewLink subMsg model

            EditProfileAction subMsg ->
                onEditProfile subMsg model

            PortalLinksAction subMsg ->
                onPortalLinksAction subMsg model

            ProviderLinksAction subMsg ->
                onUpdateProviderLinks subMsg model FromOther

            ProviderContentTypeLinksAction subMsg ->
                let
                    provider =
                        if model.portal.requested == Domain.ViewLinks then
                            ProviderContentTypeLinks.update subMsg model.portal.provider
                        else
                            ProviderContentTypeLinks.update subMsg model.selectedProvider
                in
                    case subMsg of
                        ProviderContentTypeLinks.Toggle _ ->
                            if model.portal.requested == Domain.ViewLinks then
                                ( { model | portal = { portal | provider = provider } }, Cmd.none )
                            else
                                ( { model | selectedProvider = provider }, Cmd.none )

                        ProviderContentTypeLinks.Featured _ ->
                            ( { model | portal = { portal | provider = provider } }, Cmd.none )

            ProviderTopicContentTypeLinksAction subMsg ->
                ( model, Cmd.none )

            Subscription update ->
                case update of
                    Subscribe _ _ ->
                        ( model, Cmd.none )

                    Unsubscribe _ _ ->
                        ( model, Cmd.none )

            NavigateBack ->
                ( model, Navigation.back 1 )


onUpdateProviderLinks : ProviderLinks.Msg -> Model -> Linksfrom -> ( Model, Cmd Msg )
onUpdateProviderLinks subMsg model linksfrom =
    case subMsg of
        ProviderLinks.Toggle _ ->
            let
                provider =
                    case linksfrom of
                        FromPortal ->
                            ProviderLinks.update subMsg model.portal.provider

                        FromOther ->
                            ProviderLinks.update subMsg model.selectedProvider
            in
                ( { model | selectedProvider = provider }, Cmd.none )


onPortalLinksAction : ProviderLinks.Msg -> Model -> ( Model, Cmd Msg )
onPortalLinksAction subMsg model =
    case subMsg of
        ProviderLinks.Toggle _ ->
            let
                provider =
                    ProviderLinks.update subMsg model.portal.provider

                pendingPortal =
                    model.portal
            in
                ( { model | portal = { pendingPortal | provider = provider } }, Cmd.none )


onEditProfile : EditProfile.Msg -> Model -> ( Model, Cmd Msg )
onEditProfile subMsg model =
    let
        updatedProfile =
            EditProfile.update subMsg model.portal.provider.profile

        ( portal, provider ) =
            ( model.portal, model.portal.provider )

        newState =
            { model | portal = { portal | provider = { provider | profile = updatedProfile } } }
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
                            | provider = { provider | profile = v }
                            , sourcesNavigation = True
                            , linksNavigation = not <| provider.portfolio == initPortfolio
                            , requested = Domain.ViewSources
                        }
                  }
                , Cmd.none
                )


onRegistration : Registration.Msg -> Model -> ( Model, Cmd Msg )
onRegistration subMsg model =
    let
        ( form, subCmd ) =
            Registration.update subMsg model.registration

        registrationCmd =
            Cmd.map OnRegistration subCmd
    in
        case subMsg of
            Registration.FirstNameInput _ ->
                ( { model | registration = form }, registrationCmd )

            Registration.LastNameInput _ ->
                ( { model | registration = form }, registrationCmd )

            Registration.EmailInput _ ->
                ( { model | registration = form }, registrationCmd )

            Registration.PasswordInput _ ->
                ( { model | registration = form }, registrationCmd )

            Registration.ConfirmInput _ ->
                ( { model | registration = form }, registrationCmd )

            Registration.Submit ->
                ( { model | registration = form }, registrationCmd )

            Registration.Response result ->
                case result of
                    Result.Ok jsonProfile ->
                        let
                            newUser =
                                jsonProfileToProvider jsonProfile

                            newState =
                                { model
                                    | registration = form
                                    , portal =
                                        { initPortal
                                            | provider = newUser
                                            , requested = Domain.EditProfile
                                            , linksNavigation = False
                                            , sourcesNavigation = False
                                        }
                                }
                        in
                            ( newState, Navigation.load <| "/#/portal/" ++ getId newUser.profile.id )

                    Result.Err _ ->
                        ( model, registrationCmd )


onRemove : Model -> Source -> ( Model, Cmd Msg )
onRemove model sources =
    let
        provider =
            model.portal.provider

        ( profile, pendingPortal ) =
            ( provider.profile, model.portal )

        sourcesLeft =
            profile.sources |> List.filter (\c -> c /= sources)

        updatedProvider =
            { provider | profile = updatedProfile }

        updatedProfile =
            { profile | sources = sourcesLeft }

        portal =
            { pendingPortal | provider = updatedProvider, newSource = initSource }

        newState =
            { model | portal = portal }
    in
        ( newState, Cmd.none )


refreshLinks : Provider -> List Link -> Portfolio
refreshLinks provider addedLinks =
    let
        links =
            provider.portfolio

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
        ( pendingPortal, provider ) =
            ( model.portal, model.portal.provider )

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
                            , provider = { provider | portfolio = refreshLinks provider updatedLinks.added }
                        }
                in
                    ( { model | portal = updatedPortal }, Cmd.none )


onAddedSource : AddSource.Msg -> Model -> ( Model, Cmd Msg )
onAddedSource subMsg model =
    let
        ( pendingPortal, provider, updatedProfile ) =
            ( model.portal, model.portal.provider, model.portal.provider.profile )

        addSourceModel =
            AddSource.update subMsg { source = pendingPortal.newSource, sources = provider.profile.sources }

        updatedProvider =
            { provider | profile = { updatedProfile | sources = addSourceModel.sources } }

        portal =
            { pendingPortal | newSource = addSourceModel.source, provider = updatedProvider }
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
                            | linksNavigation = portfolioExists provider.portfolio
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
                            | linksNavigation = portfolioExists portal.provider.portfolio
                            , sourcesNavigation = True
                            , addLinkNavigation = True
                        }
                  }
                , Cmd.none
                )


filterProviders : List Provider -> String -> List Provider
filterProviders providers matchValue =
    let
        isMatch name =
            name |> toLower |> contains (matchValue |> toLower)

        onFirstName provider =
            provider.profile.firstName |> getName |> isMatch

        onLastName provider =
            provider.profile.lastName |> getName |> isMatch

        onName provider =
            onFirstName provider || onLastName provider
    in
        providers |> List.filter onName


matchProviders : Model -> String -> ( Model, Cmd Msg )
matchProviders model matchValue =
    ( { model | providers = filterProviders model.providers matchValue }, Cmd.none )


onLogin : Login.Msg -> Model -> ( Model, Cmd Msg )
onLogin subMsg model =
    let
        ( login, subCmd ) =
            Login.update subMsg model.login

        ( loginCmd, pendingPortal ) =
            ( Cmd.map OnLogin subCmd, model.portal )
    in
        case subMsg of
            Login.Response result ->
                case result of
                    Result.Ok jsonProvider ->
                        let
                            provider =
                                jsonProvider |> toProvider

                            newState =
                                { model
                                    | portal =
                                        { pendingPortal
                                            | provider = provider
                                            , requested = Domain.ViewRecent
                                            , linksNavigation = portfolioExists provider.portfolio
                                            , sourcesNavigation = not <| List.isEmpty provider.profile.sources
                                        }
                                }
                        in
                            ( newState, Navigation.load <| "/#/portal/" ++ getId provider.profile.id )

                    Result.Err _ ->
                        ( { model | login = login }, loginCmd )

            Login.Attempt _ ->
                ( { model | login = login }, loginCmd )

            Login.UserInput _ ->
                ( { model | login = login }, loginCmd )

            Login.PasswordInput _ ->
                ( { model | login = login }, loginCmd )



-- VIEW


view : Model -> Html Msg
view model =
    case model.currentRoute.hash |> tokenizeUrl of
        [] ->
            homePage model

        [ "register" ] ->
            model |> renderPage (Html.map OnRegistration <| Registration.view model.registration)

        [ "provider", id ] ->
            model
                |> renderPage
                    (renderProfileBase model.selectedProvider <|
                        Html.map ProviderLinksAction (ProviderLinks.view FromOther model.selectedProvider)
                    )

        [ "provider", id, topic ] ->
            case runtime.provider <| Id id of
                _ ->
                    model |> renderPage (providerTopicPage FromOther model.selectedProvider)

        [ "provider", id, "all", contentType ] ->
            case runtime.provider <| Id id of
                _ ->
                    let
                        ( view, provider ) =
                            ( ProviderContentTypeLinks.view, model.selectedProvider )

                        contentToEmbed =
                            Html.map ProviderContentTypeLinksAction <| view provider (toContentType contentType) False
                    in
                        model |> renderPage (renderProfileBase model.selectedProvider <| contentToEmbed)

        [ "provider", id, topicName, "all", contentType ] ->
            case runtime.provider <| Id id of
                _ ->
                    let
                        topic =
                            Topic topicName False

                        contentToEmbed =
                            Html.map ProviderTopicContentTypeLinksAction <| ProviderTopicContentTypeLinks.view model.selectedProvider topic <| toContentType contentType
                    in
                        model |> renderPage (renderProfileBase model.selectedProvider <| contentToEmbed)

        [ "portal", id, "all", contentType ] ->
            let
                linksContent =
                    Html.map ProviderContentTypeLinksAction <| ProviderContentTypeLinks.view model.portal.provider (toContentType contentType) True

                contentToEmbed =
                    linksContent |> applyToPortal model.portal.provider model
            in
                model |> renderPage (model |> content (Just contentToEmbed))

        [ "portal", id ] ->
            let
                mainContent =
                    model
                        |> content Nothing
                        |> applyToPortal model.portal.provider model
            in
                model |> renderPage mainContent

        [ "portal", clientId, "provider", id ] ->
            let
                contentLinks =
                    (renderProfileBase model.selectedProvider <|
                        Html.map ProviderLinksAction (ProviderLinks.view FromOther model.selectedProvider)
                    )
            in
                model |> renderPage contentLinks

        [ "portal", clientId, "provider", id, topic ] ->
            let
                contentLinks =
                    (renderProfileBase model.selectedProvider <|
                        Html.map ProviderTopicContentTypeLinksAction (ProviderTopicContentTypeLinks.view model.selectedProvider (Topic topic False) All)
                    )
            in
                model |> renderPage contentLinks

        _ ->
            pageNotFound


renderProfileBase : Provider -> Html Msg -> Html Msg
renderProfileBase provider linksContent =
    table []
        [ tr []
            [ table []
                [ tr [ class "bio" ] [ td [] [ img [ class "profile", src <| getUrl <| provider.profile.imageUrl ] [] ] ]
                , tr [ class "bio" ] [ td [] [ text <| getName provider.profile.firstName ++ " " ++ getName provider.profile.lastName ] ]
                , tr [ class "bio" ] [ td [] [ button [ class "subscribeButton" ] [ text "Follow" ] ] ]
                , tr [ class "bio" ] [ td [] [ p [] [ text provider.profile.bio ] ] ]
                ]
            , td [] [ linksContent ]
            ]
        ]


applyToPortal : Provider -> Model -> Html Msg -> Html Msg
applyToPortal provider model content =
    let
        portal =
            model.portal
    in
        if portal.provider == initProvider then
            portal |> render provider content model.providers
        else
            portal |> render portal.provider content model.providers


render : Provider -> Html Msg -> List Provider -> Portal -> Html Msg
render provider content portal providers =
    table []
        [ tr []
            [ td []
                [ table []
                    [ tr [ class "bio" ] [ td [] [ img [ class "profile", src <| getUrl <| provider.profile.imageUrl ] [] ] ]
                    , tr [] [ td [] <| renderNavigation providers portal ]
                    ]
                ]
            , td [] [ content ]
            ]
        ]


headerContent : Model -> Html Msg
headerContent model =
    let
        loginUI : Model -> Html Msg
        loginUI model =
            let
                profileId =
                    getId model.portal.provider.profile.id

                ( loggedIn, welcome, signout, profile, sources ) =
                    ( model.login.loggedIn
                    , p [] [ text <| "Welcome " ++ model.login.email ++ "!" ]
                    , label [ class "ProfileSettings", onClick EditProfile ] [ text "Profile" ]
                    , label [ class "ProfileSettings", onClick ViewSources ] [ text "Sources" ]
                    , a [ href "" ] [ label [] [ text "Signout" ] ]
                    )
            in
                if (not loggedIn) then
                    Html.map OnLogin <| Login.view model.login
                else
                    div [ class "signin" ]
                        [ welcome
                        , signout
                        , br [] []
                        , profile
                        ]
    in
        div []
            [ header [ class "header" ]
                [ img [ class "logo", src "Assets/Nikeza_thin_2.png" ] []
                , br [] []
                , model |> loginUI
                ]
            ]


footerContent : Html Msg
footerContent =
    footer [ class "copyright" ]
        [ a [ href "" ] [ text "Lamba Cartel" ] ]


providersUI : Maybe Provider -> List Provider -> Bool -> Html Msg
providersUI loggedIn providers showSubscriptionState =
    Html.map ProfileThumbnail <|
        div [] (providers |> List.map (ProfileThumbnail.thumbnail loggedIn showSubscriptionState))


recentProvidersUI : Id -> List Provider -> Html Msg
recentProvidersUI clientId providers =
    Html.map RecentProviderLinks <|
        div [ class "mainContent" ]
            [ h3 [] [ text "Recent Links" ]
            , div [ class "mainContent" ] (providers |> List.map (\p -> RecentProviderLinks.thumbnail clientId p))
            ]


homePage : Model -> Html Msg
homePage model =
    let
        mainContent =
            table []
                [ tr [] [ td [] [ input [ class "search", type_ "text", placeholder "name", onInput Search ] [] ] ]
                , tr []
                    [ td [] [ div [] [ providersUI Nothing model.providers False ] ]
                    , td []
                        [ table []
                            [ tr []
                                [ td [] [ button [ class "join", onClick Register ] [ text "Join!" ] ]
                                , td []
                                    [ ul [ class "featuresList" ]
                                        [ li [ class "joinReasons" ] [ text "Import links to your articles, videos, and answers" ]
                                        , li [ class "joinReasons" ] [ text "Set your featured links for other viewers to see" ]
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
    let
        placeHolder =
            case model.currentRoute.hash |> tokenizeUrl of
                [] ->
                    div [] []

                [ "home" ] ->
                    div [] []

                [ "portal", _ ] ->
                    div [] []

                _ ->
                    input [ class "backbutton", type_ "image", src "Assets/BackButton.jpg", onClick NavigateBack ] []
    in
        div []
            [ headerContent model
            , placeHolder
            , content
            , footerContent
            ]


providerTopicPage : Linksfrom -> Provider -> Html Msg
providerTopicPage linksfrom model =
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
                    -- [ td [] [ div [] <| contentWithTopicUI linksfrom profileId Answer topic (runtime.topicLinks topic Answer profileId) ]
                    [ td [] [ div [] <| contentWithTopicUI linksfrom profileId Answer topic [] ]

                    -- , td [] [ div [] <| contentWithTopicUI linksfrom profileId Article topic (runtime.topicLinks topic Article profileId) ]
                    , td [] [ div [] <| contentWithTopicUI linksfrom profileId Article topic [] ]
                    ]
                , tr []
                    [ td [] [ b [] [ text "Podcasts" ] ]
                    , td [] [ b [] [ text "Videos" ] ]
                    ]
                , tr []
                    -- [ td [] [ div [] <| contentWithTopicUI linksfrom profileId Podcast topic (runtime.topicLinks topic Podcast profileId) ]
                    [ td [] [ div [] <| contentWithTopicUI linksfrom profileId Podcast topic [] ]

                    -- , td [] [ div [] <| contentWithTopicUI linksfrom profileId Video topic (runtime.topicLinks topic Video profileId) ]
                    , td [] [ div [] <| contentWithTopicUI linksfrom profileId Video topic [] ]
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
                pageNotFound


content : Maybe (Html Msg) -> Model -> Html Msg
content contentToEmbed model =
    let
        portal =
            model.portal

        loggedIn =
            portal.provider

        (Members followingYou) =
            loggedIn.followers

        (Members following) =
            loggedIn.subscriptions
    in
        case portal.requested of
            Domain.ViewSources ->
                div []
                    [ Html.map SourceAdded <|
                        AddSource.view
                            { source = portal.newSource
                            , sources = loggedIn.profile.sources
                            }
                    ]

            Domain.ViewLinks ->
                let
                    contentToDisplay =
                        case contentToEmbed of
                            Just v ->
                                v

                            Nothing ->
                                div [] [ Html.map PortalLinksAction <| ProviderLinks.view FromPortal loggedIn ]
                in
                    contentToDisplay

            Domain.EditProfile ->
                div [] [ Html.map EditProfileAction <| EditProfile.view loggedIn.profile ]

            Domain.AddLink ->
                let
                    linkSummary =
                        portal.newLinks

                    newLinkEditor =
                        Html.map NewLink (NewLinks.view (linkSummary))

                    addLink l =
                        div []
                            [ label [] [ text <| (l.contentType |> contentTypeToText |> dropRight 1) ++ ": " ]
                            , a [ href <| getUrl l.url, target "_blank" ] [ text <| getTitle l.title ]
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
                following |> searchProvidersUI (Just loggedIn) False "name of subscription"

            Domain.ViewFollowers ->
                followingYou |> searchProvidersUI (Just loggedIn) True "name of follower"

            Domain.ViewProviders ->
                loggedIn |> filteredProvidersUI model.providers "name"

            Domain.ViewRecent ->
                following |> recentLinksContent loggedIn.profile.id


recentLinks : List Provider -> List Link
recentLinks providers =
    let
        onRecentLinks provider =
            if
                provider.recentLinks
                    |> List.isEmpty
                    |> not
            then
                Just provider.recentLinks
            else
                Nothing
    in
        providers
            |> List.filterMap onRecentLinks
            |> List.concat


recentLinksContent : Id -> List Provider -> Html Msg
recentLinksContent profileId providers =
    providers
        |> List.filter (\p -> p.recentLinks /= [])
        |> recentProvidersUI profileId


removeProvider : Id -> List Provider -> List Provider
removeProvider profileId providers =
    providers |> List.filter (\p -> p.profile.id /= profileId)


filteredProvidersUI : List Provider -> String -> Provider -> Html Msg
filteredProvidersUI providers placeHolder loggedIn =
    providers
        |> removeProvider loggedIn.profile.id
        |> searchProvidersUI (Just loggedIn) True placeHolder


searchProvidersUI : Maybe Provider -> Bool -> String -> List Provider -> Html Msg
searchProvidersUI loggedIn showSubscriptionState placeHolder providers =
    table []
        [ tr [] [ td [] [ input [ class "search", type_ "text", placeholder placeHolder, onInput Search ] [] ] ]
        , tr [] [ td [] [ div [] [ providersUI (loggedIn) providers showSubscriptionState ] ] ]
        ]


renderNavigation : Portal -> List Provider -> List (Html Msg)
renderNavigation portal providers =
    let
        links =
            portal.provider.portfolio

        (Members subscriptions) =
            portal.provider.subscriptions

        (Members followers) =
            portal.provider.followers

        profile =
            portal.provider.profile

        sourcesText =
            "Sources " ++ "(" ++ (toString <| List.length profile.sources) ++ ")"

        recentCount =
            recentLinks subscriptions |> List.length

        newText =
            "Recent "
                ++ "("
                ++ (subscriptions
                        |> recentLinks
                        |> List.length
                        |> toString
                   )
                ++ ")"

        ( portfolioText, subscriptionsText, membersText, linkText, profileText ) =
            ( "Portfolio", "Subscriptions", "Members", "Link", "Profile" )

        followersText =
            "Followers " ++ "(" ++ (toString <| List.length followers) ++ ")"

        allNavigation =
            case portal.requested of
                Domain.ViewRecent ->
                    [ button [ class "selectedNavigationButton4", onClick ViewRecent ] [ text newText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewLinks ] [ text portfolioText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick AddNewLink ] [ text linkText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewSubscriptions ] [ text subscriptionsText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewFollowers ] [ text followersText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewProviders ] [ text membersText ]
                    ]

                Domain.ViewSources ->
                    [ button [ class "navigationButton4", onClick ViewRecent ] [ text newText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewLinks ] [ text portfolioText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick AddNewLink ] [ text linkText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewSubscriptions ] [ text subscriptionsText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewFollowers ] [ text followersText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewProviders ] [ text membersText ]
                    ]

                Domain.ViewLinks ->
                    [ button [ class "navigationButton4", onClick ViewRecent ] [ text newText ]
                    , br [] []
                    , br [] []
                    , button [ class "selectedNavigationButton4", onClick ViewLinks ] [ text portfolioText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick AddNewLink ] [ text linkText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewSubscriptions ] [ text subscriptionsText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewFollowers ] [ text followersText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewProviders ] [ text membersText ]
                    ]

                Domain.AddLink ->
                    [ button [ class "navigationButton4", onClick ViewRecent ] [ text newText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewLinks ] [ text portfolioText ]
                    , br [] []
                    , button [ class "selectedNavigationButton4", onClick AddNewLink ] [ text linkText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewSubscriptions ] [ text subscriptionsText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewFollowers ] [ text followersText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewProviders ] [ text membersText ]
                    ]

                Domain.EditProfile ->
                    [ button [ class "navigationButton4", onClick ViewRecent ] [ text newText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewLinks ] [ text portfolioText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick AddNewLink ] [ text linkText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewSubscriptions ] [ text subscriptionsText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewFollowers ] [ text followersText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewProviders ] [ text membersText ]
                    ]

                Domain.ViewSubscriptions ->
                    [ button [ class "navigationButton4", onClick ViewRecent ] [ text newText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewLinks ] [ text portfolioText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick AddNewLink ] [ text linkText ]
                    , br [] []
                    , br [] []
                    , button [ class "selectedNavigationButton4", onClick ViewSubscriptions ] [ text subscriptionsText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewFollowers ] [ text followersText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewProviders ] [ text membersText ]
                    , br [] []
                    , br [] []
                    ]

                Domain.ViewFollowers ->
                    [ button [ class "navigationButton4", onClick ViewRecent ] [ text newText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewLinks ] [ text portfolioText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick AddNewLink ] [ text linkText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewSubscriptions ] [ text subscriptionsText ]
                    , br [] []
                    , button [ class "selectedNavigationButton4", onClick ViewFollowers ] [ text followersText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewProviders ] [ text membersText ]
                    ]

                Domain.ViewProviders ->
                    [ button [ class "navigationButton4", onClick ViewRecent ] [ text newText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewLinks ] [ text portfolioText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick AddNewLink ] [ text linkText ]
                    , br [] []
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewSubscriptions ] [ text subscriptionsText ]
                    , br [] []
                    , button [ class "navigationButton4", onClick ViewFollowers ] [ text followersText ]
                    , br [] []
                    , button [ class "selectedNavigationButton4", onClick ViewProviders ] [ text membersText ]
                    ]

        sourcesButNoLinks =
            let
                noSelectedButton =
                    [ button [ class "navigationButton3", onClick ViewSources ] [ text sourcesText ]
                    , br [] []
                    , button [ class "navigationButton3", onClick AddNewLink ] [ text linkText ]
                    ]
            in
                case portal.requested of
                    Domain.ViewSources ->
                        [ button [ class "selectedNavigationButton3", onClick ViewSources ] [ text sourcesText ]
                        , br [] []
                        , button [ class "navigationButton3", onClick AddNewLink ] [ text linkText ]
                        ]

                    Domain.AddLink ->
                        [ button [ class "navigationButton3", onClick ViewSources ] [ text sourcesText ]
                        , br [] []
                        , button [ class "selectedNavigationButton3", onClick AddNewLink ] [ text linkText ]
                        ]

                    Domain.EditProfile ->
                        [ button [ class "navigationButton3", onClick ViewSources ] [ text sourcesText ]
                        , br [] []
                        , button [ class "navigationButton3", onClick AddNewLink ] [ text linkText ]
                        ]

                    Domain.ViewLinks ->
                        noSelectedButton

                    Domain.ViewSubscriptions ->
                        noSelectedButton

                    Domain.ViewFollowers ->
                        noSelectedButton

                    Domain.ViewProviders ->
                        noSelectedButton

                    Domain.ViewRecent ->
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


pageNotFound : Html Msg
pageNotFound =
    div [] [ text "Page not found" ]


linksUI : List Link -> List (Html Msg)
linksUI links =
    links
        |> List.take 5
        |> List.map (\link -> a [ href <| getUrl link.url, target "_blank" ] [ text <| getTitle link.title, br [] [] ])


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
        [ "provider", id ] ->
            ( { model | currentRoute = location }, runtime.provider (Id id) NavigateToProviderResponse )

        [ "provider", id, "all", contentType ] ->
            ( { model | currentRoute = location }, runtime.provider (Id id) NavigateToProviderResponse )

        [ "provider", id, topic ] ->
            let
                ( providerId, providerTopic ) =
                    ( (Id id), (Topic topic False) )
            in
                ( { model | currentRoute = location }, runtime.providerTopic providerId providerTopic NavigateToProviderTopicResponse )

        [ "portal", id ] ->
            let
                login =
                    model.login
            in
                ( { model | login = { login | loggedIn = True }, currentRoute = location }, runtime.provider (Id id) NavigateToPortalResponse )

        [ "portal", id, topic ] ->
            let
                ( providerId, providerTopic ) =
                    ( (Id id), (Topic topic False) )
            in
                ( { model | currentRoute = location }, runtime.providerTopic providerId providerTopic NavigateToPortalProviderTopicResponse )

        [ "portal", id, "all", contentType ] ->
            ( { model | currentRoute = location }, Cmd.none )

        [ "portal", clientId, "provider", id, topic ] ->
            let
                ( providerId, providerTopic ) =
                    ( (Id id), (Topic topic False) )
            in
                ( { model | currentRoute = location }, runtime.providerTopic providerId providerTopic NavigateToPortalProviderMemberTopicResponse )

        [ "portal", clientId, "provider", id ] ->
            ( { model | currentRoute = location }, runtime.provider (Id id) NavigateToPortalProviderMemberResponse )

        _ ->
            ( { model | currentRoute = location }, Cmd.none )
