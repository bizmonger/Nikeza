module Home exposing (..)

import Domain.Core exposing (..)
import Domain.Login as Login exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)


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
    { videos = [], articles = [], login = Login.model }


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
    | Login Login.Msg


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

        Login subMsg ->
            let
                newState =
                    Login.update subMsg model.login
            in
                { model | login = newState }



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ header []
            [ label [] [ text "Nikeza" ]
            , model |> sessionUI
            ]
        , footer [ class "copyright" ]
            [ label [] [ text "(c)2017" ]
            , label [] [ text "GitHub" ]
            ]
        ]


sessionUI : Model -> Html Msg
sessionUI model =
    let
        loggedIn =
            model.login.loggedIn

        welcome =
            p [] [ text <| "Welcome " ++ model.login.username ++ "!" ]

        signout =
            a [ href "" ] [ label [] [ text "Signout" ] ]
    in
        if (not loggedIn) then
            Html.map Login <| Login.view model.login
        else
            div [ class "signin" ] [ welcome, signout ]
