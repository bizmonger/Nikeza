module Domain.Login exposing (..)

import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)


-- MODEL


type alias Model =
    { username : String
    , password : String
    , loggedIn : Bool
    }


model : Model
model =
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
            { model | username = v }

        PasswordInput v ->
            { model | password = v }

        Attempt ( username, password ) ->
            if String.toLower username == "test" && String.toLower password == "test" then
                { model | loggedIn = True }
            else
                { model | loggedIn = False }



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ input [ class "signin", type_ "submit", value "Signin", onClick <| Attempt ( model.username, model.password ) ] []
        , input [ class "signin", type_ "password", placeholder "password", onInput PasswordInput, value model.password ] []
        , input [ class "signin", type_ "text", placeholder "username", onInput UserInput, value model.username ] []
        ]
