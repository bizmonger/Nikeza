module Home exposing (..)

import Domain.Core exposing (..)
import Domain.Contributor as Contributor exposing (..)
import Controls.Login as Login exposing (..)
import Controls.ProfileThumbnail as ProfileThumbnail exposing (..)
import Controls.AddConnection as AddConnection exposing (..)
import Controls.NewLinks as NewLinks exposing (..)
import Settings exposing (runtime)
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
    , contributors : List Profile
    , contributor : Contributor.Model
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
          , contributors = runtime.contributors
          , login = Login.model
          , contributor = contributor
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
    | NewLink NewLinks.Msg
    | Toggle ( Topic, Bool )
    | ToggleAll Bool
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

        Toggle ( topic, include ) ->
            ( topic, include ) |> toggleFilter model

        ToggleAll include ->
            include |> toggleAllFilter model

        ProfileThumbnail subMsg ->
            ( model, Cmd.none )

        NewConnection subMsg ->
            onNewConnection subMsg model

        Remove connection ->
            onRemove model connection

        NewLink subMsg ->
            onNewLink subMsg model


onRemove : Model -> Connection -> ( Model, Cmd Msg )
onRemove model connection =
    let
        contributor =
            model.contributor

        profile =
            contributor.profile

        connectionsLeft =
            profile.connections |> List.filter (\c -> c /= connection)

        updatedProfile =
            { profile | connections = connectionsLeft }

        newState =
            { model
                | contributor =
                    { contributor
                        | profile = updatedProfile
                        , newConnection = initConnection
                    }
            }
    in
        ( newState, Cmd.none )


onNewLink : NewLinks.Msg -> Model -> ( Model, Cmd Msg )
onNewLink subMsg model =
    let
        contributor =
            model.contributor

        newState =
            NewLinks.update subMsg (contributor.newLinks)
    in
        case subMsg of
            NewLinks.InputTitle _ ->
                ( { model | contributor = { contributor | newLinks = newState } }, Cmd.none )

            NewLinks.InputUrl _ ->
                ( { model | contributor = { contributor | newLinks = newState } }, Cmd.none )

            NewLinks.InputTopic _ ->
                ( { model | contributor = { contributor | newLinks = newState } }, Cmd.none )

            NewLinks.AssociateTopic _ ->
                ( { model | contributor = { contributor | newLinks = newState } }, Cmd.none )

            NewLinks.InputContentType _ ->
                ( { model | contributor = { contributor | newLinks = newState } }, Cmd.none )

            AddLink v ->
                ( { model | contributor = { contributor | newLinks = { newState | canAdd = True, added = v.current :: v.added } } }, Cmd.none )


onNewConnection : AddConnection.Msg -> Model -> ( Model, Cmd Msg )
onNewConnection subMsg model =
    let
        connection =
            AddConnection.update subMsg model.contributor.newConnection

        contributor =
            model.contributor
    in
        case subMsg of
            AddConnection.InputUsername _ ->
                ( { model | contributor = { contributor | newConnection = connection } }, Cmd.none )

            AddConnection.InputPlatform _ ->
                ( { model | contributor = { contributor | newConnection = connection } }, Cmd.none )

            AddConnection.Submit _ ->
                let
                    profile =
                        contributor.profile
                in
                    ( { model
                        | contributor =
                            { contributor
                                | newConnection = connection
                                , profile = { profile | connections = connection :: profile.connections }
                            }
                      }
                    , Cmd.none
                    )


toggleAllFilter : Model -> Bool -> ( Model, Cmd Msg )
toggleAllFilter model include =
    let
        contributor =
            model.contributor

        profile =
            contributor.profile

        updateContributor =
            if not include then
                { contributor | showAll = False, answers = [], articles = [], videos = [], podcasts = [] }
            else
                { contributor
                    | showAll = True
                    , answers = profile.id |> runtime.links Answer
                    , articles = profile.id |> runtime.links Article
                    , videos = profile.id |> runtime.links Video
                    , podcasts = profile.id |> runtime.links Podcast
                }
    in
        ( { model | contributor = updateContributor }, Cmd.none )


toggleFilter : Model -> ( Topic, Bool ) -> ( Model, Cmd Msg )
toggleFilter model ( topic, include ) =
    let
        contributor =
            model.contributor

        toggleTopic contentType links =
            if include then
                List.append (contributor.profile.id |> runtime.topicLinks topic contentType) links
            else
                links |> List.filter (\l -> not (l.topics |> List.member topic))

        newState =
            { model
                | contributor =
                    { contributor
                        | showAll = False
                        , answers = contributor.answers |> toggleTopic Answer
                        , articles = contributor.articles |> toggleTopic Article
                        , videos = contributor.videos |> toggleTopic Video
                        , podcasts = contributor.podcasts |> toggleTopic Podcast
                    }
            }
    in
        ( newState, Cmd.none )


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
    case subMsg of
        Login.Attempt _ ->
            let
                login =
                    Login.update subMsg model.login

                ( contributor, latest ) =
                    ( model.contributor, runtime.tryLogin login )

                newState =
                    case runtime.contributor <| runtime.usernameToId login.username of
                        Just p ->
                            { model
                                | login = latest
                                , contributor =
                                    { contributor
                                        | profile = { p | connections = p.id |> runtime.connections }
                                    }
                            }

                        Nothing ->
                            { model | login = latest }
            in
                if newState.login.loggedIn then
                    ( newState, Navigation.load <| "/#/" ++ getId newState.contributor.profile.id ++ "/dashboard" )
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
                    contributorPage model.contributor

                Nothing ->
                    notFoundPage

        [ "contributor", id, topic ] ->
            case runtime.contributor <| Id id of
                Just _ ->
                    contributorTopicPage model.contributor

                Nothing ->
                    notFoundPage

        [ "contributor", id, "all", contentType ] ->
            case runtime.contributor <| Id id of
                Just _ ->
                    contributorContentTypePage contentType model.contributor

                Nothing ->
                    notFoundPage

        [ "contributor", id, topic, "all", contentType ] ->
            case runtime.contributor <| Id id of
                Just _ ->
                    contributorTopicContentTypePage (Topic topic) (toContentType contentType) model.contributor

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


contributorPage : Contributor.Model -> Html Msg
contributorPage model =
    let
        ( profileId, topics ) =
            ( model.profile.id, model.profile.topics )

        allTopic =
            Topic "All"

        allFilter =
            div []
                [ input [ type_ "checkbox", checked model.showAll, onCheck (\b -> ToggleAll b) ] []
                , label [] [ text <| getTopic allTopic ]
                ]

        toCheckBoxState include topic =
            div []
                [ input [ type_ "checkbox", checked include, onCheck (\isChecked -> Toggle ( topic, isChecked )) ] []
                , label [] [ text <| getTopic topic ]
                ]
    in
        div []
            [ table []
                [ tr []
                    [ table []
                        [ tr []
                            [ td [] [ img [ src <| getUrl <| model.profile.imageUrl, width 100, height 100 ] [] ]

                            -- , td [] [ div [] <| allFilter :: (topics |> List.map (\t -> t |> toCheckBoxState model.showAll)) ]
                            , td [] [ div [] <| (topics |> List.map (\t -> t |> toCheckBoxState True)) ]
                            , table []
                                [ tr []
                                    [ td [] [ b [] [ text "Answers" ] ]
                                    , td [] [ b [] [ text "Articles" ] ]
                                    ]
                                , tr []
                                    [ td [] [ div [] <| contentUI profileId Answer model.answers ]
                                    , td [] [ div [] <| contentUI profileId Article model.articles ]
                                    ]
                                , tr []
                                    [ td [] [ b [] [ text "Podcasts" ] ]
                                    , td [] [ b [] [ text "Videos" ] ]
                                    ]
                                , tr []
                                    [ td [] [ div [] <| contentUI profileId Podcast model.podcasts ]
                                    , td [] [ div [] <| contentUI profileId Video model.videos ]
                                    ]
                                ]
                            ]
                        , tr [] [ td [] [ text <| getName model.profile.name ] ]
                        , tr [] [ td [] [ p [] [ text model.profile.bio ] ] ]
                        ]
                    ]
                ]
            ]


contributorContentTypePage : String -> Contributor.Model -> Html Msg
contributorContentTypePage contentTypeText model =
    let
        topics =
            model.profile.topics

        posts =
            case contentTypeText |> toContentType of
                Answer ->
                    model.answers

                Article ->
                    model.articles

                Podcast ->
                    model.podcasts

                Video ->
                    model.videos

                Unknown ->
                    []

                All ->
                    []
    in
        div []
            [ h2 [] [ text <| "All " ++ contentTypeText ]
            , table []
                [ tr []
                    [ td [] [ img [ src <| getUrl <| model.profile.imageUrl, width 100, height 100 ] [] ]
                    , td [] [ div [] (topics |> List.map toCheckbox) ]
                    , td [] [ div [] <| List.map (\link -> a [ href <| getUrl link.url ] [ text <| getTitle link.title, br [] [] ]) posts ]
                    ]
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


dashboardPage : Model -> Html Msg
dashboardPage model =
    let
        contributor =
            model.contributor

        connectionsTable =
            table [] [ div [] (contributor.profile.connections |> List.map connectionUI) ]

        linkSummary =
            contributor.newLinks

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
        div []
            [ h2 [] [ text <| "Welcome " ++ getName model.contributor.profile.name ]
            , div []
                [ h3 [] [ text "Connections" ]
                , Html.map NewConnection <| AddConnection.view model.contributor.newConnection
                , connectionsTable
                ]
            , h3 [] [ text "Add Link" ]
            , Html.map NewLink (NewLinks.view linkSummary)
            , update
            ]


notFoundPage : Html Msg
notFoundPage =
    div [] [ text "Page not found" ]


toCheckbox : Topic -> Html Msg
toCheckbox topic =
    div []
        [ input [ type_ "checkbox", checked True, onCheck (\b -> Toggle ( topic, b )) ] []
        , label [] [ text <| getTopic topic ]
        ]


linksUI : List Link -> List (Html Msg)
linksUI links =
    links
        |> List.take 5
        |> List.map (\link -> a [ href <| getUrl link.url ] [ text <| getTitle link.title, br [] [] ])


contentUI : Id -> ContentType -> List Link -> List (Html Msg)
contentUI profileId contentType links =
    List.append (linksUI links) [ a [ href <| getUrl <| moreContributorContentUrl profileId contentType ] [ text <| "all", br [] [] ] ]


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
                    ( { model | contributor = getContributor profile, currentRoute = location }, Cmd.none )

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
                        ( { model | contributor = topicContributor, currentRoute = location }, Cmd.none )

                Nothing ->
                    ( { model | currentRoute = location }, Cmd.none )

        _ ->
            ( { model | currentRoute = location }, Cmd.none )
