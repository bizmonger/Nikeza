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



-- UPDATE


update : Msg -> Model -> Model
update msg model =
    model



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ h3 [] [ text "Profile" ]
        , input [ type_ "text", placeholder "name", onInput NameInput ] []
        , br [] []
        , input [ type_ "text", placeholder "email", onInput EmailInput ] []
        , br [] []
        , textarea [ placeholder "bio..." ] []
        , br [] []
        , button [] [ text "Save" ]
        ]
