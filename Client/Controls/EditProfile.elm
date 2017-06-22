module Controls.EditProfile exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onInput)


-- MODEL


type alias Model =
    Profile


type Msg
    = FirstNameInput String
    | LastNameInput String
    | EmailInput String
    | BioInput String
    | Save Model



-- UPDATE


update : Msg -> Model -> Model
update msg model =
    case msg of
        FirstNameInput v ->
            { model | firstName = Name v }

        LastNameInput v ->
            { model | lastName = Name v }

        EmailInput v ->
            { model | email = Email v }

        BioInput v ->
            { model | bio = v }

        Save v ->
            v



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ input [ type_ "text", placeholder "first name", onInput FirstNameInput, value <| getName model.firstName ] []
        , br [] []
        , input [ type_ "text", placeholder "last name", onInput LastNameInput, value <| getName model.lastName ] []
        , br [] []
        , input [ type_ "text", placeholder "email", onInput EmailInput, value <| getEmail model.email ] []
        , br [] []
        , textarea [ placeholder "bio description", onInput BioInput, value model.bio ] []
        , br [] []
        , button [ onClick <| Save model ] [ text "Save" ]
        ]
