module Controls.EditProfile exposing (..)

import Settings exposing (..)
import Domain.Core exposing (..)
import Services.Adapter exposing (..)
import Http
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onInput)
import Navigation exposing (..)


type Msg
    = FirstNameInput String
    | LastNameInput String
    | EmailInput String
    | BioInput String
    | Update
    | Response (Result Http.Error JsonProfile)



-- UPDATE


update : Msg -> Profile -> ( Profile, Cmd Msg )
update msg profile =
    case msg of
        FirstNameInput v ->
            ( { profile | firstName = Name v }, Cmd.none )

        LastNameInput v ->
            ( { profile | lastName = Name v }, Cmd.none )

        EmailInput v ->
            ( { profile | email = Email v }, Cmd.none )

        BioInput v ->
            ( { profile | bio = v }, Cmd.none )

        Update ->
            ( profile, (runtime.updateProfile profile) Response )

        Response (Ok jsonProfile) ->
            ( jsonProfile |> toProfile, Cmd.none )

        Response (Err error) ->
            Debug.crash (toString error) ( profile, Cmd.none )



-- VIEW


view : Profile -> Html Msg
view profile =
    div [ class "mainContent" ]
        [ h3 [] [ text "Profile" ]
        , input [ class "profileInput", type_ "text", placeholder "first name", onInput FirstNameInput, value <| nameText profile.firstName ] []
        , br [] []
        , input [ class "profileInput", type_ "text", placeholder "last name", onInput LastNameInput, value <| nameText profile.lastName ] []
        , br [] []
        , input [ class "profileInput", type_ "text", placeholder "email", onInput EmailInput, value <| emailText profile.email ] []
        , br [] []
        , textarea [ class "inputBio", placeholder "bio description", onInput BioInput, value profile.bio ] []
        , br [] []
        , button [ class "saveProfile", onClick <| Update ] [ text "Save" ]
        ]
