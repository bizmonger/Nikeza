module Home exposing (..)

import Domain.Core exposing (..)
import Domain.Contributor as Contributor exposing (..)
import Controls.Login as Login exposing (..)
import Controls.ProfileThumbnail as ProfileThumbnail exposing (..)
import Settings exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onCheck)
import Navigation exposing (..)


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
    , contributors : List Contributor
    , login : Login.Model
    , contributor : Contributor.Model
    }


init : Navigation.Location -> ( Model, Cmd Msg )
init location =
    ( { currentRoute = location
      , contributors = []
      , login = Login.model
      , contributor =
            case tokenizeUrl location.hash of
                [ "contributor", id ] ->
                    case runtime.contributor <| Id id of
                        Just p ->
                            getContributor p

                        Nothing ->
                            Contributor.init

                _ ->
                    Contributor.init
      }
    , Cmd.none
    )



-- UPDATE


type Msg
    = UrlChange Navigation.Location
    | OnLogin Login.Msg
    | ProfileThumbnail ProfileThumbnail.Msg
    | Toggle ( Topic, Bool )
    | Search String
    | Register


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        UrlChange location ->
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

        OnLogin subMsg ->
            onLogin model subMsg

        Search v ->
            ( model, Cmd.none )

        Register ->
            ( model, Cmd.none )

        Toggle ( topic, include ) ->
            let
                contributor =
                    model.contributor

                toggleTopic contentType links =
                    if include then
                        List.append (contributor.profile.id |> runtime.topicLinks topic contentType) links
                    else
                        links |> List.filter (\a -> not (a.topics |> List.member topic))

                newState =
                    { model
                        | contributor =
                            { contributor
                                | answers = contributor.answers |> toggleTopic Answer
                                , articles = contributor.articles |> toggleTopic Article
                                , videos = contributor.videos |> toggleTopic Video
                                , podcasts = contributor.podcasts |> toggleTopic Podcast
                            }
                    }
            in
                ( newState, Cmd.none )

        ProfileThumbnail subMsg ->
            ( model, Cmd.none )


getContributor : Profile -> Contributor.Model
getContributor p =
    { profile = p
    , topics = p.topics
    , answers = p.id |> runtime.links Answer
    , articles = p.id |> runtime.links Article
    , videos = p.id |> runtime.links Video
    , podcasts = p.id |> runtime.links Podcast
    }


onLogin : Model -> Login.Msg -> ( Model, Cmd Msg )
onLogin model subMsg =
    case subMsg of
        Login.Attempt v ->
            let
                latest =
                    Login.update subMsg model.login
            in
                ( { model | login = runtime.tryLogin latest }, Cmd.none )

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

        _ ->
            notFoundPage


toCheckbox : Topic -> Html Msg
toCheckbox topic =
    div []
        [ input [ type_ "checkbox", checked True, onCheck (\b -> Toggle ( topic, b )) ] [ text <| getTopic topic ]
        , label [] [ text <| getTopic topic ]
        ]


linkSet : List Link -> List (Html Msg)
linkSet links =
    links
        |> List.take 5
        |> List.map (\link -> a [ href <| getUrl link.url ] [ text <| getTitle link.title, br [] [] ])


contentUI : Id -> ContentType -> List Link -> List (Html Msg)
contentUI profileId contentType links =
    List.append (linkSet links) [ a [ href <| getUrl <| moreContributorContentUrl profileId contentType ] [ text <| "more...", br [] [] ] ]


contentWithTopicUI : Id -> ContentType -> Topic -> List Link -> List (Html Msg)
contentWithTopicUI profileId contentType topic links =
    List.append (linkSet links) [ a [ href <| getUrl <| moreContributorContentOnTopicUrl profileId contentType topic ] [ text <| "more...", br [] [] ] ]


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
            Html.map ProfileThumbnail (div [] (runtime.recentContributors |> List.map thumbnail))
    in
        div []
            [ header []
                [ label [] [ text "Nikeza" ]
                , model |> loginUI
                ]
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
    in
        div []
            [ table []
                [ tr []
                    [ table []
                        [ tr []
                            [ td [] [ img [ src <| getUrl <| model.profile.imageUrl, width 100, height 100 ] [] ]
                            , td [] [ div [] (topics |> List.map toCheckbox) ]
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
        ( profileId, topics ) =
            ( model.profile.id, model.profile.topics )

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


notFoundPage : Html Msg
notFoundPage =
    div [] [ text "Page not found" ]



-- NAVIGATION


type alias RoutePath =
    List String


tokenizeUrl : String -> RoutePath
tokenizeUrl urlHash =
    urlHash |> String.split "/" |> List.drop 1
