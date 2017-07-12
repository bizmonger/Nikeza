module Controls.Login exposing (..)

import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)


-- MODEL


type alias Model =
    { email : String
    , password : String
    , loggedIn : Bool
    }


init : Model
init =
    Model "" "" False



-- UPDATE


type Msg
    = UserInput String
    | PasswordInput String
    | Attempt ( String, String )


update : Msg -> Model -> Model
update msg model =
    case msg of
        UserInput v ->
            { model | email = v }

        PasswordInput v ->
            { model | password = v }

        Attempt ( email, password ) ->
            { model | email = email, password = password }



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ input [ class "signin", type_ "submit", tabindex 3, value "Signin", onClick <| Attempt ( model.email, model.password ) ] []
        , input [ class "signin", type_ "password", tabindex 2, placeholder "password", onInput PasswordInput, value model.password ] []
        , input [ class "signin", type_ "text", tabindex 1, placeholder "username", onInput UserInput, value model.email ] []
        ]
