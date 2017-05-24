module Home exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)


main =
    Html.beginnerProgram
        { model = model
        , update = update
        , view = view
        }



-- MODEL


type alias Model =
    { videos : List Video, articles : List Article }


model : Model
model =
    { videos = [], articles = [] }


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

        SignIn username password ->
            model



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ header []
            [ label [ class "title" ] [ text "Nikeza" ]
            , input [ class "signin", type_ "submit", value "Signin" ] []
            , input [ class "signin", type_ "password", placeholder "password" ] []
            , input [ class "signin", type_ "text", placeholder "username" ] []
            ]
        , footer []
            [ label [] [ text "(c)2017" ]
            , label [] [ text "GitHub" ]
            ]
        ]
