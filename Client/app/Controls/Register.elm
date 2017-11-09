module Controls.Register exposing (..)

import Settings exposing (..)
import Domain.Core exposing (Form)
import Services.Adapter exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import Http
import Navigation exposing (..)


type Msg
    = FirstNameInput String
    | LastNameInput String
    | EmailInput String
    | PasswordInput String
    | ConfirmInput String
    | Submit
    | Response (Result Http.Error JsonProfile)


update : Msg -> Form -> ( Form, Cmd Msg )
update msg form =
    case msg of
        FirstNameInput v ->
            ( { form | firstName = v }, Cmd.none )

        LastNameInput v ->
            ( { form | lastName = v }, Cmd.none )

        EmailInput v ->
            ( { form | email = v }, Cmd.none )

        PasswordInput v ->
            ( { form | password = v }, Cmd.none )

        ConfirmInput v ->
            ( { form | confirm = v }, Cmd.none )

        Submit ->
            ( form, runtime.tryRegister form Response )

        Response (Ok jsonProfile) ->
            ( form, Navigation.load <| "/#/portal/" ++ jsonProfile.id )

        Response (Err error) ->
            Debug.crash (toString error) ( form, Cmd.none )



-- VIEW


view : Form -> Html Msg
view form =
    div [ class "RegistrationForm" ]
        -- [ input [ class "registrationInput", type_ "text", placeholder "first name", onInput FirstNameInput, value form.firstName ] []
        -- , br [] []
        -- , input [ class "registrationInput", type_ "text", placeholder "last name", onInput LastNameInput, value form.lastName ] []
        -- , br [] []
        [ input [ class "registrationEmail", type_ "email", placeholder "email", onInput EmailInput, value form.email ] []
        , br [] []
        , input [ class "registrationPassword", type_ "password", placeholder "password", onInput PasswordInput, value form.password ] []
        , br [] []
        , input [ class "registrationPassword", type_ "password", placeholder "confirm", onInput ConfirmInput, value form.confirm ] []
        , br [] []
        , br [] []
        , button [ class "register", value "Create Account", onClick Submit ] [ text "Join" ]
        ]
