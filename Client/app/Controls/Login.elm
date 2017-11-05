module Controls.Login exposing (..)

import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import Http
import Navigation exposing (..)
import Domain.Core exposing (..)
import Settings exposing (..)
import Services.Adapter exposing (..)


type Msg
    = UserInput String
    | PasswordInput String
    | Attempt ( String, String )
    | Response (Result Http.Error JsonProvider)


update : Msg -> Credentials -> ( Credentials, Cmd Msg )
update msg credentials =
    case msg of
        UserInput v ->
            ( { credentials | email = v }, Cmd.none )

        PasswordInput v ->
            ( { credentials | password = v }, Cmd.none )

        Attempt ( email, password ) ->
            ( credentials, runtime.tryLogin credentials Response )

        Response (Ok jsonProvider) ->
            let
                (JsonProvider jsonProviderField) =
                    jsonProvider
            in
                ( credentials, Navigation.load <| "/#/portal/" ++ jsonProviderField.profile.id )

        Response (Err error) ->
            Debug.crash ("Error: " ++ toString error) ( credentials, Cmd.none )



-- VIEW


view : Credentials -> Html Msg
view model =
    div []
        [ input [ class "signin", type_ "submit", tabindex 3, value "Signin", onClick <| Attempt ( model.email, model.password ) ] []
        , input [ class "signin", type_ "password", tabindex 2, placeholder "password", onInput PasswordInput, value model.password ] []
        , input [ class "signin", type_ "text", tabindex 1, placeholder "username", onInput UserInput, value model.email ] []
        ]
