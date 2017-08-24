module Controls.Register exposing (..)

import Settings exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import Http
import Domain.Core exposing (JsonProfile, Form)
import Navigation exposing (..)


-- COMMANDS


type Msg
    = FirstNameInput String
    | LastNameInput String
    | EmailInput String
    | PasswordInput String
    | ConfirmInput String
    | Submit
    | Response (Result Http.Error JsonProfile)


update : Msg -> Form -> ( Form, Cmd Msg )
update msg model =
    case msg of
        FirstNameInput v ->
            ( { model | firstName = v }, Cmd.none )

        LastNameInput v ->
            ( { model | lastName = v }, Cmd.none )

        EmailInput v ->
            ( { model | email = v }, Cmd.none )

        PasswordInput v ->
            ( { model | password = v }, Cmd.none )

        ConfirmInput v ->
            ( { model | confirm = v }, Cmd.none )

        Submit ->
            ( model, runtime.tryRegister model Response )

        Response (Ok json) ->
            ( model, Navigation.load <| "/#/portal/1" )

        Response (Err error) ->
            ( model, Cmd.none )



-- VIEW


view : Form -> Html Msg
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
            , onClick Submit
            ]
            [ text "Join" ]
        ]
