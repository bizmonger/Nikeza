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
    | Contributor Contributor.Msg
    | Video Video
    | Article Article
    | Search String
    | Register
    | ProfileThumbnail ProfileThumbnail.Msg


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
    let
        routePath =
            fromUrlHash model.currentRoute.hash
    in
        case routePath of
            [] ->
                homePage model

            [ "home" ] ->
                homePage model

            [ "contributor", id ] ->
                Html.map Contributor <| Contributor.view <| Contributor.Model (Id "") [] [] [] []

            _ ->
                notFoundPage


homePage : Model -> Html Msg
homePage model =
    div []
        [ header []
            [ label [] [ text "Nikeza" ]
            , model |> renderLogin
            ]
        , div [] [ contributors ]
        , footer [ class "copyright" ]
            [ label [] [ text "(c)2017" ]
            , a [ href "" ] [ text "GitHub" ]
            ]
        ]


notFoundPage : Html Msg
notFoundPage =
    div [] [ text "Not Found" ]


contributors : Html Msg
contributors =
    Html.map ProfileThumbnail (div [] (runtime.recentContributors |> List.map thumbnail))


renderLogin : Model -> Html Msg
renderLogin model =
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


fromUrlHash : String -> RoutePath
fromUrlHash urlHash =
    urlHash |> String.split "/" |> List.drop 1
