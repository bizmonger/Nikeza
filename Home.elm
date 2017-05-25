module Home exposing (..)

import Domain.Core exposing (..)
import Domain.Login as Login exposing (..)
import Html exposing (..)


main =
    Html.beginnerProgram
        { model = model
        , update = update
        , view = view
        }



-- MODEL


type alias Model =
    { videos : List Video
    , articles : List Article
    , login : Login.Model
    }


model : Model
model =
    { videos = [], articles = [], login = Login.Model "" "" }


init : ( Model, Cmd Msg )
init =
    ( model, Cmd.none )



-- UPDATE


type Msg
    = Video Video
    | Article Article
    | Submitter Submitter
    | Search String
    | Register
    | HandleLogin Login.Msg


update : Msg -> Model -> Model
update msg model =
    case msg of
        Video v ->
            model

        Article v ->
            model

        Submitter v ->
            model

        Search v ->
            model

        Register ->
            model

        HandleLogin subMsg ->
            let
                newState =
                    Login.update subMsg model.login
            in
                { model | login = newState }



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ header [] [ Html.map HandleLogin <| Login.view model.login ]
        , footer []
            [ label [] [ text "(c)2017" ]
            , label [] [ text "GitHub" ]
            ]
        ]
