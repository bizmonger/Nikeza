module Home exposing (..)

import Domain.Core exposing (..)
import Domain.Contributor as Contributor exposing (..)
import Controls.Login as Login exposing (..)
import Controls.ProfileThumbnail as ProfileThumbnail exposing (..)
import Settings exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Navigation exposing (..)


-- elm-live Home.elm --open --output=home.js
-- elm-make Home.elm --output=home.html
-- elm-package install elm-lang/navigation


main =
    Navigation.program UrlChange
        { init = model
        , view = view
        , update = update
        , subscriptions = (\_ -> Sub.none)
        }



-- MODEL


type alias Model =
    { currentRoute : Navigation.Location
    , content : Content
    , contributors : List Contributor
    , login : Login.Model
    }


type alias Content =
    { videos : List Video
    , articles : List Article
    , podcasts : List Podcast
    }


model : Navigation.Location -> ( Model, Cmd Msg )
model location =
    ( { currentRoute = location
      , content = Content [] [] []
      , contributors = []
      , login = Login.model
      }
    , Cmd.none
    )



-- UPDATE


type Msg
    = UrlChange Navigation.Location
    | OnLogin Login.Msg
    | ProfileThumbnail ProfileThumbnail.Msg
    | Contributor Contributor.Msg
    | Video Video
    | Article Article
    | Search String
    | Register


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        UrlChange location ->
            ( { model | currentRoute = location }, Cmd.none )

        OnLogin subMsg ->
            onLogin model subMsg

        Video v ->
            ( model, Cmd.none )

        Article v ->
            ( model, Cmd.none )

        Search v ->
            ( model, Cmd.none )

        Register ->
            ( model, Cmd.none )

        Contributor subMsg ->
            ( model, Cmd.none )

        ProfileThumbnail subMsg ->
            ( model, Cmd.none )


videos : Id -> List Video
videos profileId =
    runtime.videos profileId


articles : Id -> List Article
articles profileId =
    runtime.articles profileId


podcasts : Id -> List Podcast
podcasts profileId =
    runtime.podcasts profileId


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
                Just p ->
                    Html.map Contributor <| Contributor.view <| Contributor.Model p [] [] [] []

                Nothing ->
                    notFoundPage

        _ ->
            notFoundPage


homePage : Model -> Html Msg
homePage model =
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


notFoundPage : Html Msg
notFoundPage =
    div [] [ text "Not Found" ]


contributorsUI : Html Msg
contributorsUI =
    Html.map ProfileThumbnail (div [] (runtime.recentContributors |> List.map thumbnail))


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



-- NAVIGATION


type alias RoutePath =
    List String


tokenizeUrl : String -> RoutePath
tokenizeUrl urlHash =
    urlHash |> String.split "/" |> List.drop 1
