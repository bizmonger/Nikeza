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
                    case runtime.getContributor <| Id id of
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
    | Contributor
    | Toggle ( Topic, Bool )
    | Search String
    | Register


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        UrlChange location ->
            case tokenizeUrl location.hash of
                [ "contributor", id ] ->
                    case runtime.getContributor <| Id id of
                        Just profile ->
                            ( { model | contributor = getContributor profile, currentRoute = location }, Cmd.none )

                        Nothing ->
                            ( { model | currentRoute = location }, Cmd.none )

                [ "contributor", id, topic ] ->
                    case runtime.getContributor <| Id id of
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

        Contributor ->
            ( model, Cmd.none )

        Toggle ( topic, include ) ->
            let
                contributor =
                    model.contributor

                toggleTopic contentType posts =
                    if include then
                        List.append (contributor.profile.id |> runtime.topicPosts topic contentType) posts
                    else
                        posts |> List.filter (\a -> not (a.topics |> List.member topic))

                newState =
                    { model
                        | contributor =
                            { contributor
                                | articles = contributor.articles |> toggleTopic Article
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
    , articles = p.id |> runtime.posts Article
    , videos = p.id |> runtime.posts Video
    , podcasts = p.id |> runtime.posts Podcast
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
            case runtime.getContributor <| Id id of
                Just _ ->
                    contributorPage model.contributor

                Nothing ->
                    notFoundPage

        [ "contributor", id, topic ] ->
            case runtime.getContributor <| Id id of
                Just _ ->
                    contributorTopicPage model.contributor

                Nothing ->
                    notFoundPage

        [ "contributor", id, "all", contentType ] ->
            case runtime.getContributor <| Id id of
                Just _ ->
                    contributorContentTypePage contentType model.contributor

                Nothing ->
                    notFoundPage

        _ ->
            notFoundPage


contentUI : Id -> ContentType -> List Post -> List (Html Msg)
contentUI profileId contentType posts =
    let
        recentLinks =
            posts
                |> List.take 5
                |> List.map (\post -> a [ href <| getUrl post.url ] [ text <| getTitle post.title, br [] [] ])
    in
        List.append recentLinks [ a [ href <| getUrl <| moreContributorContentUrl profileId contentType ] [ text <| "more...", br [] [] ] ]


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
        profileId =
            model.profile.id

        topicTocheckbox : Topic -> Html Msg
        topicTocheckbox topic =
            div []
                [ input [ type_ "checkbox", checked True, onCheck (\b -> Toggle ( topic, b )) ] [ text <| getTopic topic ]
                , label [] [ text <| getTopic topic ]
                ]

        topicsUI : List Topic -> Html Msg
        topicsUI topics =
            let
                formattedTopics =
                    topics |> List.map topicTocheckbox
            in
                div [] formattedTopics
    in
        div []
            [ table []
                [ tr []
                    [ table []
                        [ tr []
                            [ td [] [ img [ src <| getUrl <| model.profile.imageUrl, width 100, height 100 ] [] ]
                            , td [] [ topicsUI model.profile.topics ]
                            , table []
                                [ tr [] [ td [] [ b [] [ text "Videos" ] ] ]
                                , div [] <| contentUI profileId Video model.videos
                                , tr [] [ td [] [ b [] [ text "Podcasts" ] ] ]
                                , div [] <| contentUI profileId Podcast model.podcasts
                                , tr [] [ td [] [ b [] [ text "Articles" ] ] ]
                                , div [] <| contentUI profileId Article model.articles
                                ]
                            ]
                        , tr [] [ td [] [ text <| getName model.profile.name ] ]
                        , tr [] [ td [] [ p [] [ text model.profile.bio ] ] ]
                        ]
                    ]
                ]
            ]


contributorContentTypePage : String -> Contributor.Model -> Html Msg
contributorContentTypePage contentType model =
    h2 [] [ text <| "All " ++ contentType ]


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
                                        , div [] <| contentUI profileId Video (runtime.topicPosts topic Video profileId)
                                        , tr [] [ td [] [ b [] [ text "Podcasts" ] ] ]
                                        , div [] <| contentUI profileId Podcast (runtime.topicPosts topic Podcast profileId)
                                        , tr [] [ td [] [ b [] [ text "Articles" ] ] ]
                                        , div [] <| contentUI profileId Article (runtime.topicPosts topic Article profileId)
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
