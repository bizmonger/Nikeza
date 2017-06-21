module Controls.EditProfile exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onInput)


-- MODEL


type alias Model =
    Profile


type Msg
    = NameInput String
    | EmailInput String
    | BioInput String
    | Save Model



-- UPDATE


update : Msg -> Model -> Model
update msg model =
    case msg of
        NameInput v ->
            { model | name = Name v }

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
        [ h3 [] [ text "Profile" ]
        , input [ type_ "text", placeholder "name", onInput NameInput, value <| getName model.name ] []
        , br [] []
        , input [ type_ "text", placeholder "email", onInput EmailInput, value <| getEmail model.email ] []
        , br [] []
        , textarea [ placeholder "bio...", onInput BioInput, value model.bio ] []
        , br [] []
        , button [ onClick <| Save model ] [ text "Save" ]
        ]
