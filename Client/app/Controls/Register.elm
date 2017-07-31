module Controls.Register exposing (..)

import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)


-- MODEL


type alias Model =
    { firstName : String
    , lastName : String
    , email : String
    , password : String
    , confirm : String
    }


model : Model
model =
    Model "" "" "" "" ""



-- UPDATE


type Msg
    = FirstNameInput String
    | LastNameInput String
    | EmailInput String
    | PasswordInput String
    | ConfirmInput String
    | Submit Model


update : Msg -> Model -> Model
update msg model =
    case msg of
        FirstNameInput v ->
            { model | firstName = v }

        LastNameInput v ->
            { model | lastName = v }

        EmailInput v ->
            { model | email = v }

        PasswordInput v ->
            { model | password = v }

        ConfirmInput v ->
            { model | confirm = v }

        Submit v ->
            v



-- VIEW


view : Model -> Html Msg
view model =
    div [ class "RegistrationForm" ]
        [ input [ type_ "text", placeholder "first name", onInput FirstNameInput, value model.firstName ] []
        , br [] []
        , input [ type_ "text", placeholder "last name", onInput LastNameInput, value model.lastName ] []
        , br [] []
        , input [ type_ "email", placeholder "email", onInput EmailInput, value model.email ] []
        , br [] []
        , input [ type_ "password", placeholder "password", onInput PasswordInput, value model.password ] []
        , br [] []
        , input [ type_ "password", placeholder "confirm", onInput ConfirmInput, value model.confirm ] []
        , br [] []
        , br [] []
        , button
            [ class "register"
            , value "Create Account"
            , onClick <|
                Submit
                    { firstName = model.firstName
                    , lastName = model.lastName
                    , email = model.email
                    , password = model.password
                    , confirm = model.confirm
                    }
            ]
            [ text "Join" ]
        ]
