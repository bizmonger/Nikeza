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
    | Save ( Name, Email, String )



-- UPDATE


update : Msg -> Model -> Model
update msg model =
    model



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ h3 [] [ text "Profile" ]
        , input [ type_ "text", placeholder "name", value <| getName model.name, onInput NameInput ] []
        , br [] []
        , input [ type_ "text", placeholder "email", value <| getEmail model.email, onInput EmailInput ] []
        , br [] []
        , textarea [ placeholder "bio...", value model.bio ] []
        , br [] []
        , button [ onClick <| Save ( model.name, model.email, model.bio ) ] [ text "Save" ]
        ]
