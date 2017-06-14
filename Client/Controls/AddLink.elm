module Controls.AddLink exposing (..)

import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)


-- MODEL


type alias Model =
    Link


type Msg
    = TitleInput String
    | UrlInput String
    | TagsInput List String
    | AddLink Model



-- UPDATE


update : Msg -> Model -> Model
update msg model =
    model



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ input [ type_ "text", placeholder "title" ] []
        , input [ type_ "text", placeholder "link" ] []
        , select []
            [ option [ value "undefined" ] [ text "Select Type" ]
            , option [ value "Article" ] [ text "Article" ]
            , option [ value "Video" ] [ text "Video" ]
            , option [ value "Answer" ] [ text "Answer" ]
            , option [ value "Podcast" ] [ text "Podcast" ]
            ]
        , button [ onClick <| AddLink model ] [ text "Add" ]
        ]
