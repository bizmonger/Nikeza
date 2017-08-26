module Controls.Login exposing (..)

import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import Http
import Navigation exposing (..)
import Domain.Core exposing (JsonProfile)


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
    | Response (Result Http.Error JsonProfile)


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case msg of
        UserInput v ->
            ( { model | email = v }, Cmd.none )

        PasswordInput v ->
            ( { model | password = v }, Cmd.none )

        Attempt ( email, password ) ->
            ( { model | email = email, password = password }, Cmd.none )

        Response (Ok jsonProfile) ->
            ( model, Navigation.load <| "/#/portal/" ++ jsonProfile.id )

        Response (Err error) ->
            ( model, Cmd.none )



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ input [ class "signin", type_ "submit", tabindex 3, value "Signin", onClick <| Attempt ( model.email, model.password ) ] []
        , input [ class "signin", type_ "password", tabindex 2, placeholder "password", onInput PasswordInput, value model.password ] []
        , input [ class "signin", type_ "text", tabindex 1, placeholder "username", onInput UserInput, value model.email ] []
        ]
