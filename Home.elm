module Home exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)


main =
    Html.beginnerProgram
        { model = model
        , update = update
        , view = view
        }



-- MODEL


type alias Model =
    { videos : List Video
    , articles : List Article
    , login : Credentials
    }


model : Model
model =
    { videos = [], articles = [], login = Credentials "" "" }


init : ( Model, Cmd Msg )
init =
    ( model, Cmd.none )



-- UPDATE


type Msg
    = Video Video
    | Article Article
    | Submitter Submitter
    | Search String
    | Register
    | UserName String
    | Password String
    | SignIn String String


update : Msg -> Model -> Model
update msg model =
    case msg of
        Video v ->
            model

        Article v ->
            model

        Submitter v ->
            model

        Search v ->
            model

        Register ->
            model

        UserName v ->
            model

        Password v ->
            model

        SignIn username password ->
            model



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ span []
            [ label [ class "title" ] [ text "Nikeza" ]
            , input [ class "signin", type_ "submit", value "Signin", onClick <| SignIn model.login.username model.login.password ] []
            , input [ class "signin", type_ "password", placeholder "password", onInput Password, value model.login.password ] []
            , input [ class "signin", type_ "text", placeholder "username", onInput UserName, value model.login.username ] []
            ]
        , footer []
            [ label [] [ text "(c)2017" ]
            , label [] [ text "GitHub" ]
            ]
        ]
