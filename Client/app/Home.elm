module Home exposing (..)

import Settings exposing (runtime)
import Domain.Core as Domain exposing (..)
import Controls.Login as Login exposing (..)
import Controls.ProfileThumbnail as ProfileThumbnail exposing (..)
import Controls.RecentProviderLinks as RecentProviderLinks exposing (..)
import Controls.Sources as Sources exposing (..)
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
    , platforms : List Platform
    , portal : Portal
    , providers : List Provider
    , scopedProviders : List Provider
    , searchResult : List Provider
    , selectedProvider : Provider
    }


init : Navigation.Location -> ( Model, Cmd Msg )
init location =
    ( { currentRoute = location
      , login = initCredentials
      , registration = initForm
      , platforms = []
      , portal = initPortal
      , providers = []
      , scopedProviders = []
      , searchResult = []
      , selectedProvider = initProvider
      }
    , runtime.bootstrap BootstrapResponse
    )



-- UPDATE


type Msg
    = UrlChange Navigation.Location
    | ViewSources
    | AddNewLink
    | ViewLinks
    | EditProfile
    | ViewSubscriptions
    | ViewFollowers
    | ViewProviders
    | ViewRecent
    | OnLogin Login.Msg
    | ProfileThumbnail ProfileThumbnail.Msg
    | RecentProviderLinks RecentProviderLinks.Msg
    | SourcesUpdated Sources.Msg
    | NewLink NewLinks.Msg
    | ProviderLinksAction ProviderLinks.Msg
    | PortalLinksAction ProviderLinks.Msg
    | EditProfileAction EditProfile.Msg
    | ProviderContentTypeLinksAction ProviderContentTypeLinks.Msg
    | ProviderTopicContentTypeLinksAction ProviderTopicContentTypeLinks.Msg
    | ThumbnailResponse (Result Http.Error JsonThumbnail)
    | SaveThumbnailResponse (Result Http.Error String)
    | ProvidersResponse (Result Http.Error (List JsonProvider))
    | BootstrapResponse (Result Http.Error JsonBootstrap)
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

        provider =
            portal.provider

        profile =
            provider.profile
    in
        case msg of
            UrlChange location ->
                location |> navigate msg model

            SaveThumbnailResponse response ->
                case response of
                    Ok _ ->
                        ( model, Cmd.none )

                    Err _ ->
                        ( model, Cmd.none )

            ThumbnailResponse response ->
                case response of
                    Ok thumbnail ->
                        let
                            updatedProfile =
                                { profile | imageUrl = Url thumbnail.imageUrl }

                            updatedProvider =
                                { provider | profile = updatedProfile }

                            updatedPortal =
                                { portal | provider = updatedProvider }

                            request =
                                { profileId = profile.id, imageUrl = Url thumbnail.imageUrl }
                        in
                            ( { model | portal = updatedPortal }, runtime.updateThumbnail request SaveThumbnailResponse )

                    Err _ ->
                        ( model, Cmd.none )

            ProvidersResponse response ->
                case response of
                    Ok jsonProviders ->
                        let
                            providers =
                                jsonProviders |> List.map (\p -> p |> toProvider)
                        in
                            ( { model
                                | providers = providers
                                , searchResult = providers
                                , scopedProviders = providers
                              }
                            , Cmd.none
                            )

                    Err _ ->
                        ( model, Cmd.none )

            BootstrapResponse response ->
                case response of
                    Ok bootstrap ->
                        let
                            providers =
                                bootstrap.providers |> List.map toProvider
                        in
                            ( { model
                                | providers = providers
                                , scopedProviders = providers
                                , searchResult = providers
                                , platforms = bootstrap.platforms |> List.map (\p -> Platform p)
                              }
                            , Cmd.none
                            )

                    Err description ->
                        Debug.crash (toString description) ( model, Cmd.none )

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
                ( { model
                    | searchResult = model.providers
                    , scopedProviders = model.providers
                  }
                , runtime.providers ProvidersResponse
                )

            Search text ->
                let
                    result =
                        model.scopedProviders |> matchProviders text
                in
                    ( { model | searchResult = result }, Cmd.none )

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
                ( { model
                    | scopedProviders = portal |> getSubscriptions
                    , portal = { portal | requested = Domain.ViewSubscriptions }
                  }
                , Cmd.none
                )

            ViewFollowers ->
                ( { model
                    | scopedProviders = portal |> getFollowers
                    , portal = { portal | requested = Domain.ViewFollowers }
                  }
                , Cmd.none
                )

            ViewProviders ->
                ( { model
                    | scopedProviders = model.providers
                    , portal = { portal | requested = Domain.ViewProviders }
                  }
                , Cmd.none
                )

            ViewRecent ->
                ( { model
                    | scopedProviders = portal |> getSubscriptions
                    , portal = { portal | requested = Domain.ViewRecent }
                  }
                , Cmd.none
                )

            SourcesUpdated subMsg ->
                onSourcesUpdated subMsg model

            NewLink subMsg ->
                onNewLink subMsg model

            EditProfileAction subMsg ->
                onEditProfile subMsg model

            PortalLinksAction subMsg ->
                onPortalLinksAction subMsg model

            ProviderLinksAction subMsg ->
                onUpdateProviderLinks subMsg model FromOther

            ProviderContentTypeLinksAction subMsg ->
                onUpdateProviderContentTypeLinks subMsg model FromOther

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


onUpdateProviderContentTypeLinks : ProviderContentTypeLinks.Msg -> Model -> Linksfrom -> ( Model, Cmd Msg )
onUpdateProviderContentTypeLinks subMsg model linksfrom =
    let
        portal =
            model.portal

        provider =
            case linksfrom of
                FromPortal ->
                    ProviderContentTypeLinks.update subMsg model.portal.provider

                FromOther ->
                    ProviderContentTypeLinks.update subMsg model.selectedProvider
    in
        case subMsg of
            ProviderContentTypeLinks.Toggle _ ->
                ( { model | selectedProvider = provider }, Cmd.none )

            ProviderContentTypeLinks.Featured _ ->
                ( { model | portal = { portal | provider = provider } }, Cmd.none )


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
        ( portal, provider ) =
            ( model.portal, model.portal.provider )

        ( updatedProfile, subCmd ) =
            EditProfile.update subMsg provider.profile

        editCmd =
            Cmd.map EditProfileAction subCmd

        newState =
            { model | portal = { portal | provider = { provider | profile = updatedProfile } } }
    in
        case subMsg of
            EditProfile.FirstNameInput _ ->
                ( newState, editCmd )

            EditProfile.LastNameInput _ ->
                ( newState, editCmd )

            EditProfile.EmailInput _ ->
                ( newState, editCmd )

            EditProfile.BioInput _ ->
                ( newState, editCmd )

            EditProfile.Update ->
                ( newState, editCmd )

            EditProfile.Response result ->
                case result of
                    Result.Ok jsonProfile ->
                        let
                            updatedModel =
                                { newState
                                    | portal =
                                        { portal
                                            | provider = { provider | profile = jsonProfile |> toProfile }
                                            , sourcesNavigation = True
                                            , linksNavigation = not <| provider.portfolio == initPortfolio
                                            , requested = Domain.ViewSources
                                        }
                                }
                        in
                            ( updatedModel, editCmd )

                    Result.Err _ ->
                        ( model, editCmd )


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
                            ( newState, Navigation.load <| "/#/portal/" ++ idText newUser.profile.id )

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

        pendingProvider =
            { provider | profile = updatedProfile }

        updatedProfile =
            { profile | sources = sourcesLeft }

        portal =
            { pendingPortal | provider = pendingProvider, newSource = initSource }

        newState =
            { model | portal = portal }
    in
        ( newState, Cmd.none )


updatePortfolio : Provider -> List Link -> Portfolio
updatePortfolio provider addedLinks =
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

        pendingNewLinks =
            pendingPortal.newLinks

        ( newLinks, subCmd ) =
            NewLinks.update subMsg { pendingNewLinks | profileId = provider.profile.id }

        newLinkCmd =
            Cmd.map NewLink subCmd

        portal =
            { pendingPortal | newLinks = newLinks }
    in
        case subMsg of
            NewLinks.InputTitle _ ->
                ( { model | portal = portal }, newLinkCmd )

            NewLinks.InputUrl _ ->
                ( { model | portal = portal }, newLinkCmd )

            NewLinks.InputTopic _ ->
                ( { model | portal = portal }, newLinkCmd )

            NewLinks.RemoveTopic _ ->
                ( { model | portal = portal }, newLinkCmd )

            NewLinks.AddTopic _ ->
                ( { model | portal = portal }, newLinkCmd )

            NewLinks.TopicSuggestionResponse (Ok _) ->
                ( { model | portal = portal }, newLinkCmd )

            NewLinks.TopicSuggestionResponse (Err _) ->
                ( { model | portal = portal }, newLinkCmd )

            NewLinks.InputContentType _ ->
                ( { model | portal = portal }, newLinkCmd )

            NewLinks.AddLink _ ->
                ( model, newLinkCmd )

            NewLinks.Response result ->
                case result of
                    Result.Ok jsonLink ->
                        let
                            ( articles, answers, videos, podcasts ) =
                                ( provider.filteredPortfolio.articles
                                , provider.filteredPortfolio.answers
                                , provider.filteredPortfolio.videos
                                , provider.filteredPortfolio.podcasts
                                )

                            updateFilter contentType contentTypeLinks =
                                let
                                    updatedContentTypeLinks =
                                        (jsonLink |> toLink) :: contentTypeLinks

                                    filteredPortfolio =
                                        provider.filteredPortfolio
                                in
                                    case contentType of
                                        Article ->
                                            { filteredPortfolio | articles = updatedContentTypeLinks }

                                        Answer ->
                                            { filteredPortfolio | answers = updatedContentTypeLinks }

                                        Video ->
                                            { filteredPortfolio | videos = updatedContentTypeLinks }

                                        Podcast ->
                                            { filteredPortfolio | podcasts = updatedContentTypeLinks }

                                        All ->
                                            provider.filteredPortfolio

                                        Unknown ->
                                            provider.filteredPortfolio

                            newFilteredPortfolio =
                                let
                                    contentType =
                                        jsonLink.contentType |> toContentType
                                in
                                    articles |> updateFilter contentType

                            updatedPortal =
                                { portal
                                    | newLinks = initNewLinks
                                    , linksNavigation = True
                                    , provider =
                                        { provider
                                            | portfolio = updatePortfolio provider newLinks.added
                                            , filteredPortfolio = newFilteredPortfolio
                                        }
                                }
                        in
                            ( { model | portal = updatedPortal }, newLinkCmd )

                    Result.Err reason ->
                        Debug.crash (toString reason) ( model, newLinkCmd )


onSourcesUpdated : Sources.Msg -> Model -> ( Model, Cmd Msg )
onSourcesUpdated subMsg model =
    let
        pendingPortal =
            model.portal

        provider =
            pendingPortal.provider

        profile =
            provider.profile

        source =
            pendingPortal.newSource

        ( sources, subCmd ) =
            Sources.update subMsg
                { profileId = profile.id
                , platforms = model.platforms
                , source = { source | profileId = model.portal.provider.profile.id }
                , sources = model.portal.provider.profile.sources
                }

        sourceCmd =
            Cmd.map SourcesUpdated subCmd

        pendingProvider =
            { provider | profile = { profile | sources = sources.sources } }

        portal =
            { pendingPortal | newSource = sources.source, provider = pendingProvider }
    in
        case subMsg of
            Sources.InputUsername _ ->
                ( { model | portal = portal }, sourceCmd )

            Sources.InputPlatform _ ->
                ( { model | portal = portal }, sourceCmd )

            Sources.Add _ ->
                ( model, sourceCmd )

            Sources.Remove _ ->
                ( { model
                    | portal =
                        { portal
                            | linksNavigation = portfolioExists portal.provider.portfolio
                            , addLinkNavigation = True
                        }
                  }
                , sourceCmd
                )

            Sources.AddResponse result ->
                case result of
                    Ok jsonSource ->
                        let
                            source =
                                jsonSource |> toSource

                            portfolio =
                                source.links |> updatePortfolio provider

                            updatedProvider =
                                { pendingProvider | portfolio = portfolio, filteredPortfolio = portfolio }
                        in
                            ( { model
                                | portal =
                                    { portal
                                        | provider = updatedProvider
                                        , linksNavigation = portfolioExists portfolio
                                        , addLinkNavigation = True
                                    }
                              }
                            , runtime.thumbnail (Platform jsonSource.platform) jsonSource.username <| ThumbnailResponse
                            )

                    Err reason ->
                        Debug.crash (toString reason) ( model, sourceCmd )

            Sources.RemoveResponse result ->
                case result of
                    Ok _ ->
                        ( model, sourceCmd )

                    Err reason ->
                        Debug.crash (toString reason) ( model, sourceCmd )


filterProviders : String -> List Provider -> List Provider
filterProviders matchValue providers =
    let
        isMatch name =
            name |> toLower |> contains (matchValue |> toLower)

        onFirstName provider =
            provider.profile.firstName |> nameText |> isMatch

        onLastName provider =
            provider.profile.lastName |> nameText |> isMatch

        onName provider =
            onFirstName provider || onLastName provider
    in
        providers |> List.filter onName


matchProviders : String -> List Provider -> List Provider
matchProviders matchValue providers =
    providers |> filterProviders matchValue


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
                            ( newState, Navigation.load <| "/#/portal/" ++ idText provider.profile.id )

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
            model |> renderPage (providerTopicPage FromOther model.selectedProvider)

        [ "provider", id, "all", contentType ] ->
            let
                contentToEmbed =
                    Html.map ProviderContentTypeLinksAction <| ProviderContentTypeLinks.view model.selectedProvider (toContentType contentType) False
            in
                model |> renderPage (renderProfileBase model.selectedProvider <| contentToEmbed)

        [ "provider", id, topicName, "all", contentType ] ->
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
                [ tr [ class "bio" ] [ td [] [ img [ class "profile", src <| urlText <| provider.profile.imageUrl ] [] ] ]
                , tr [ class "bio" ] [ td [] [ label [ class "profileName" ] [ text <| nameText provider.profile.firstName ++ " " ++ nameText provider.profile.lastName ] ] ]
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
                    [ tr [ class "bio" ] [ td [] [ img [ class "profile", src <| urlText <| provider.profile.imageUrl ] [] ] ]
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
                    idText model.portal.provider.profile.id

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
        [ a [ href "mailto:scott.nimrod@bizmonger.net" ] [ text "Bizmonger" ] ]


providersUI : Maybe Provider -> Bool -> List Provider -> Html Msg
providersUI loggedIn showSubscriptionState providers =
    Html.map ProfileThumbnail <|
        div [] (providers |> List.map (ProfileThumbnail.thumbnail loggedIn showSubscriptionState))


recentProvidersUI : Id -> List Provider -> Html Msg
recentProvidersUI clientId providers =
    Html.map RecentProviderLinks <|
        div [ class "mainContent" ]
            [ h3 [ class "portalTopicHeader" ] [ text "Recent Links" ]
            , div [ class "mainContent" ] (providers |> List.map (\p -> RecentProviderLinks.thumbnail clientId p))
            ]


homePage : Model -> Html Msg
homePage model =
    let
        mainContent =
            div []
                [ table []
                    [ tr [] [ td [] [ input [ class "search", type_ "text", placeholder "name", onInput Search ] [] ] ]
                    , tr []
                        [ td [ class "providers" ] [ div [] [ model.searchResult |> providersUI Nothing False ] ]
                        , td [ class "joinForm" ] [ Html.map OnRegistration <| Registration.view model.registration ]
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
                [ tr [] [ h2 [] [ text <| topicText topic ] ]
                , tr []
                    [ td [] [ b [] [ text "Answers" ] ]
                    , td [] [ b [] [ text "Articles" ] ]
                    ]
                , tr []
                    -- [ td [] [ div [] <| contentWithTopicUI linksfrom profileId Answer topic (runtime.topicLinks topic Answer profileId) ]
                    [ td [ class "portfolioContent" ] [ div [] <| contentWithTopicUI linksfrom profileId Answer topic [] ]

                    -- , td [] [ div [] <| contentWithTopicUI linksfrom profileId Article topic (runtime.topicLinks topic Article profileId) ]
                    , td [ class "portfolioContent" ] [ div [] <| contentWithTopicUI linksfrom profileId Article topic [] ]
                    ]
                , tr []
                    [ td [] [ b [] [ text "Podcasts" ] ]
                    , td [] [ b [] [ text "Videos" ] ]
                    ]
                , tr []
                    -- [ td [] [ div [] <| contentWithTopicUI linksfrom profileId Podcast topic (runtime.topicLinks topic Podcast profileId) ]
                    [ td [ class "portfolioContent" ] [ div [] <| contentWithTopicUI linksfrom profileId Podcast topic [] ]

                    -- , td [] [ div [] <| contentWithTopicUI linksfrom profileId Video topic (runtime.topicLinks topic Video profileId) ]
                    , td [ class "portfolioContent" ] [ div [] <| contentWithTopicUI linksfrom profileId Video topic [] ]
                    ]
                ]
    in
        case List.head model.topics of
            Just topic ->
                table []
                    [ tr []
                        [ td []
                            [ table []
                                [ tr [ class "bio" ] [ td [] [ img [ class "profile", src <| urlText <| model.profile.imageUrl ] [] ] ]
                                , tr [ class "bio" ] [ td [] [ text <| nameText model.profile.firstName ++ " " ++ nameText model.profile.lastName ] ]
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

        followingYou =
            portal |> getFollowers

        following =
            portal |> getSubscriptions
    in
        case portal.requested of
            Domain.ViewSources ->
                div []
                    [ Html.map SourcesUpdated <|
                        Sources.view
                            { profileId = portal.provider.profile.id
                            , platforms = model.platforms
                            , source = portal.newSource
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
                            , a [ href <| urlText l.url, target "_blank" ] [ text <| titleText l.title ]
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
                model.searchResult |> searchProvidersUI (Just loggedIn) False "name on subscription"

            Domain.ViewFollowers ->
                model.searchResult |> searchProvidersUI (Just loggedIn) True "name of follower"

            Domain.ViewProviders ->
                model.searchResult |> filteredProvidersUI loggedIn "name"

            Domain.ViewRecent ->
                model.searchResult |> recentLinksContent loggedIn.profile.id


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


filteredProvidersUI : Provider -> String -> List Provider -> Html Msg
filteredProvidersUI loggedIn placeHolder providers =
    providers
        |> removeProvider loggedIn.profile.id
        |> searchProvidersUI (Just loggedIn) True placeHolder


searchProvidersUI : Maybe Provider -> Bool -> String -> List Provider -> Html Msg
searchProvidersUI loggedIn showSubscriptionState placeHolder providers =
    table []
        [ tr [] [ td [] [ input [ class "search", type_ "text", placeholder placeHolder, onInput Search ] [] ] ]
        , tr [] [ td [] [ div [] [ providers |> providersUI (loggedIn) showSubscriptionState ] ] ]
        ]


renderNavigation : Portal -> List Provider -> List (Html Msg)
renderNavigation portal providers =
    let
        links =
            portal.provider.portfolio

        subscriptions =
            portal |> getSubscriptions

        followers =
            portal |> getFollowers

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

        enableOnlySourcesAndLinks =
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
            displayNavigation enableOnlySourcesAndLinks
        else if portal.sourcesNavigation && not portal.linksNavigation then
            displayNavigation enableOnlySourcesAndLinks
        else
            displayNavigation allNavigation


pageNotFound : Html Msg
pageNotFound =
    div [] [ text "Page not found" ]


linksUI : List Link -> List (Html Msg)
linksUI links =
    links
        |> List.take 5
        |> List.map
            (\link ->
                let
                    title =
                        formatTitle link
                in
                    a [ href <| urlText link.url, target "_blank" ] [ text title, br [] [] ]
            )


contentWithTopicUI : Linksfrom -> Id -> ContentType -> Topic -> List Link -> List (Html Msg)
contentWithTopicUI linksFrom profileId contentType topic links =
    List.append (linksUI links) [ a [ href <| urlText <| allTopicContentUrl linksFrom profileId contentType topic ] [ text <| "all", br [] [] ] ]



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

                portal =
                    model.portal

                provider =
                    portal.provider

                profile =
                    provider.profile

                updatedProfile =
                    { profile | id = Id id }
            in
                ( { model
                    | login = { login | loggedIn = True }
                    , portal = { portal | provider = { provider | profile = updatedProfile } }
                    , currentRoute = location
                  }
                , runtime.provider (Id id) NavigateToPortalResponse
                )

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
