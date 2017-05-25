module Domain.Login exposing (..)

import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)


-- MODEL


type alias Model =
    { username : String, password : String }


model : Model
model =
    Model "" ""



-- UPDATE


type Msg
    = UserInput String
    | PasswordInput String
    | SignIn ( String, String )


update : Msg -> Model -> Model
update msg model =
    case msg of
        UserInput v ->
            model

        PasswordInput v ->
            model

        SignIn ( username, password ) ->
            model



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ label [ class "title" ] [ text "Nikeza" ]
        , input [ class "signin", type_ "submit", value "Signin", onClick <| SignIn ( model.username, model.password ) ] []
        , input [ class "signin", type_ "password", placeholder "password", onInput PasswordInput, value model.password ] []
        , input [ class "signin", type_ "text", placeholder "username", onInput UserInput, value model.username ] []
        ]
