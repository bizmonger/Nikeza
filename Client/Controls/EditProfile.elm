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
    | BioInput String



-- UPDATE


update : Msg -> Model -> Model
update msg model =
    model



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ h3 [] [ text "Edit Profile" ]
        , input [ type_ "text", placeholder "name", onInput NameInput ] []
        , br [] []
        , textarea [ placeholder "bio..." ] []
        , br [] []
        , button [] [ text "Save" ]
        ]
