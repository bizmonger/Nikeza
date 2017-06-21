module Controls.Register exposing (..)

import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)


-- MODEL


type alias Model =
    { name : String
    , email : String
    , password : String
    , confirm : String
    }


model : Model
model =
    Model "" "" "" ""



-- UPDATE


type Msg
    = NameInput String
    | EmailInput String
    | PasswordInput String
    | ConfirmInput String
    | Submit ( String, String, String )


update : Msg -> Model -> Model
update msg model =
    case msg of
        NameInput v ->
            { model | name = v }

        EmailInput v ->
            { model | email = v }

        PasswordInput v ->
            { model | password = v }

        ConfirmInput v ->
            { model | confirm = v }

        Submit ( email, password, confirm ) ->
            { model
                | email = email
                , password = password
                , confirm = confirm
            }



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ input [ type_ "text", placeholder "name", onInput EmailInput, value model.email ] []
        , br [] []
        , input [ type_ "email", placeholder "email", onInput EmailInput, value model.email ] []
        , br [] []
        , input [ type_ "password", placeholder "password", onInput PasswordInput, value model.password ] []
        , br [] []
        , input [ type_ "password", placeholder "confirm", onInput ConfirmInput, value model.confirm ] []
        , br [] []
        , input [ type_ "submit", value "Create Account", onClick <| Submit ( model.email, model.password, model.confirm ) ] []
        ]
