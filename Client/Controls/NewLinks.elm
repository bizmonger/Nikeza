module Controls.NewLinks exposing (..)

import Settings exposing (..)
import Domain.Core exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import Json.Decode exposing (map)


-- MODEL


init : Model
init =
    { current = initLinkToCreate, canAdd = False, added = [] }


type alias Model =
    NewLinks


type Msg
    = InputTitle String
    | InputUrl String
    | InputTopic String
    | InputContentType String
    | AddLink Model
    | AssociateTopic Topic



-- UPDATE


update : Msg -> Model -> Model
update msg model =
    let
        linkToCreate =
            model.current

        linkToCreateBase =
            model.current.base
    in
        case msg of
            InputTitle v ->
                { model | current = { linkToCreate | base = { linkToCreateBase | title = Title v } } }

            InputUrl v ->
                { model | current = { linkToCreate | base = { linkToCreateBase | url = Url v } } }

            InputTopic v ->
                { model | current = { linkToCreate | currentTopic = Topic v } }

            AssociateTopic v ->
                { model | current = { linkToCreate | base = { linkToCreateBase | topics = v :: linkToCreateBase.topics } } }

            InputContentType v ->
                { model | current = { linkToCreate | base = { linkToCreateBase | contentType = toContentType v } } }

            AddLink v ->
                --v
                model



-- VIEW


view : Model -> Html Msg
view model =
    let
        toButton topic =
            div []
                [ button [ onClick <| AssociateTopic topic ] [ text <| getTopic topic ]
                , br [] []
                ]

        topicsSelectionUI search =
            div []
                (search
                    |> getTopic
                    |> runtime.suggestedTopics
                    |> List.map toButton
                )

        current =
            model.current
    in
        div []
            [ input [ type_ "text", placeholder "title", onInput InputTitle, value <| getTitle current.base.title ] []
            , input [ type_ "text", placeholder "link", onInput InputUrl, value <| getUrl current.base.url ] []
            , br [] []
            , input [ type_ "text", placeholder "topic", onInput InputTopic, value (getTopic current.currentTopic) ] []
            , select [ Html.Events.on "change" (Json.Decode.map InputContentType Html.Events.targetValue) ]
                [ option [ value "Undefined" ] [ text "Select Type" ]
                , option [ value "Article" ] [ text "Article" ]
                , option [ value "Video" ] [ text "Video" ]
                , option [ value "Answer" ] [ text "Answer" ]
                , option [ value "Podcast" ] [ text "Podcast" ]
                ]
            , br [] []
            , topicsSelectionUI current.currentTopic
            , button [ onClick <| AddLink model ] [ text "Add" ]
            ]
