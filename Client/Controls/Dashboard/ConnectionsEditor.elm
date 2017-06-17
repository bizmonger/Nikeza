module Controls.Dashboard.ConnectionsEditor exposing (..)

import Domain.Core exposing (..)
import Contributor exposing (..)
import Controls.AddConnection as AddConnection exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)


-- MODEL


type Msg
    = NewConnection AddConnection.Msg
    | Remove Connection


type alias Model =
    Contributor.Model



-- UPDATE


update : Msg -> Model -> Model
update msg model =
    model


view : Model -> Html Msg
view model =
    let
        connectionsTable =
            table [] [ div [] (contributor.profile.connections |> List.map connectionUI) ]

        connectionUI : Connection -> Html Msg
        connectionUI connection =
            tr []
                [ td [] [ text connection.platform ]
                , td [] [ i [] [ text connection.username ] ]
                , td [] [ button [ onClick <| Remove connection ] [ text "Disconnect" ] ]
                ]
    in
        table []
            [ tr []
                [ th [] [ h3 [] [ text "Connections" ] ] ]
            , tr []
                [ td [] [ Html.map NewConnection <| AddConnection.view model.contributor.newConnection ] ]
            , tr []
                [ td [] [ connectionsTable ] ]
            ]
