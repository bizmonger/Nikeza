module Controls.Register exposing (..)

import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)


-- MODEL


type alias Model =
    { email : String
    , username : String
    , password : String
    , confirm : String
    }


model : Model
model =
    Model "" "" "" ""



-- UPDATE


type Msg
    = EmailInput String
    | UserInput String
    | PasswordInput String
    | ConfirmInput String
    | Submit ( String, String, String, String )


update : Msg -> Model -> Model
update msg model =
    case msg of
        EmailInput v ->
            { model | email = v }

        UserInput v ->
            { model | username = v }

        PasswordInput v ->
            { model | password = v }

        ConfirmInput v ->
            { model | confirm = v }

        Submit ( email, username, password, confirm ) ->
            { model
                | email = email
                , username = username
                , password = password
                , confirm = confirm
            }



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ input [ class "signin", type_ "submit", value "Signin", onClick <| Submit ( model.email, model.username, model.password, model.confirm ) ] []
        , input [ class "signin", type_ "confirm", placeholder "confirm password", onInput ConfirmInput, value model.confirm ] []
        , input [ class "signin", type_ "password", placeholder "password", onInput PasswordInput, value model.password ] []
        , input [ class "signin", type_ "text", placeholder "username", onInput UserInput, value model.username ] []
        ]
