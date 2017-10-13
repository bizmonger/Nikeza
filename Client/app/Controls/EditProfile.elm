module Controls.EditProfile exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onInput)


type Msg
    = FirstNameInput String
    | LastNameInput String
    | EmailInput String
    | BioInput String
    | Save Profile



-- UPDATE


update : Msg -> Profile -> Profile
update msg profile =
    case msg of
        FirstNameInput v ->
            { profile | firstName = Name v }

        LastNameInput v ->
            { profile | lastName = Name v }

        EmailInput v ->
            { profile | email = Email v }

        BioInput v ->
            { profile | bio = v }

        Save v ->
            v



-- VIEW


view : Profile -> Html Msg
view profile =
    div [ class "mainContent" ]
        [ h3 [] [ text "Profile" ]
        , input [ type_ "text", placeholder "first name", onInput FirstNameInput, value <| nameText profile.firstName ] []
        , br [] []
        , input [ type_ "text", placeholder "last name", onInput LastNameInput, value <| nameText profile.lastName ] []
        , br [] []
        , input [ type_ "text", placeholder "email", onInput EmailInput, value <| emailText profile.email ] []
        , br [] []
        , textarea [ placeholder "bio description", onInput BioInput, value profile.bio ] []
        , br [] []
        , button [ onClick <| Save profile ] [ text "Save" ]
        ]
