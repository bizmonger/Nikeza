module Controls.EditProfile exposing (..)

import Settings exposing (..)
import Domain.Core exposing (..)
import Services.Adapter exposing (..)
import Http
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (onClick, onInput)


type Msg
    = FirstNameInput String
    | LastNameInput String
    | EmailInput String
    | InputTopic String
    | KeyDown Int
    | RemoveTopic Topic
    | AddTopic Topic
    | TopicSuggestionResponse (Result Http.Error (List String))
    | Update
    | Response (Result Http.Error JsonProvider)



-- UPDATE


update : Msg -> ProfileEditor -> ( ProfileEditor, Cmd Msg )
update msg model =
    let
        provider =
            model.provider

        profile =
            provider.profile

        onAddTopic topic =
            ( { model
                | currentTopic = Topic "" False
                , topicSuggestions = []
                , chosenTopics = topic :: model.chosenTopics
              }
            , Cmd.none
            )
    in
        case msg of
            FirstNameInput v ->
                let
                    updatedProfile =
                        { profile | firstName = Name v }

                    updatedProvider =
                        { provider | profile = updatedProfile }
                in
                    ( { model | provider = updatedProvider }, Cmd.none )

            LastNameInput v ->
                let
                    updatedProfile =
                        { profile | lastName = Name v }

                    updatedProvider =
                        { provider | profile = updatedProfile }
                in
                    ( { model | provider = updatedProvider }, Cmd.none )

            EmailInput v ->
                let
                    updatedProfile =
                        { profile | email = Email v }

                    updatedProvider =
                        { provider | profile = updatedProfile }
                in
                    ( { model | provider = updatedProvider }, Cmd.none )

            InputTopic "" ->
                let
                    currentTopic =
                        Topic "" False
                in
                    ( { model | currentTopic = currentTopic }, Cmd.none )

            InputTopic v ->
                ( { model | currentTopic = Topic v True }, runtime.suggestedTopics v TopicSuggestionResponse )

            KeyDown key ->
                if key == 13 then
                    case model.topicSuggestions of
                        topic :: _ ->
                            onAddTopic topic

                        _ ->
                            ( model, Cmd.none )
                else
                    ( model, Cmd.none )

            TopicSuggestionResponse (Ok topics) ->
                let
                    suggestions =
                        topics |> List.map (\t -> Topic t False)
                in
                    ( { model | topicSuggestions = suggestions }, Cmd.none )

            TopicSuggestionResponse (Err reason) ->
                Debug.crash (toString reason) ( model, Cmd.none )

            RemoveTopic v ->
                let
                    topics =
                        model.chosenTopics |> List.filter (\t -> t /= v)
                in
                    ( { model | chosenTopics = topics }, Cmd.none )

            AddTopic v ->
                case model.topicSuggestions of
                    topic :: _ ->
                        onAddTopic topic

                    _ ->
                        ( model, Cmd.none )

            Update ->
                let
                    profileAndTopics =
                        { profile = provider.profile, topics = model.chosenTopics }
                in
                    ( model, (runtime.updateProfileAndTopics profileAndTopics) Response )

            Response (Ok jsonProvider) ->
                ( { model | provider = jsonProvider |> toProvider }, Cmd.none )

            Response (Err error) ->
                Debug.crash (toString error) ( profile, Cmd.none )



-- VIEW


view : ProfileEditor -> Html Msg
view model =
    let
        provider =
            model.provider

        profile =
            provider.profile

        toButton topic =
            div []
                [ button [ class "topicsButton", onClick <| AddTopic topic ] [ text <| topicText topic ]
                , br [] []
                ]

        suggestionsUI textItems =
            let
                buttonsContainer =
                    textItems
                        |> List.map (\textItem -> Topic textItem False)
                        |> List.map (\t -> t |> toButton)
            in
                div [] buttonsContainer

        selectedTopicsUI =
            model.chosenTopics
                |> List.map
                    (\t ->
                        div []
                            [ button [ class "topicsButton" ] [ text <| topicText t ]
                            , button [ class "removeTopic", onClick <| RemoveTopic t ] [ text "X" ]
                            , br [] []
                            ]
                    )
    in
        div [ class "mainContent" ]
            [ h3 [ class "portalTopicHeader" ] [ text "Profile" ]
            , table [ class "editProfile" ]
                [ tr []
                    [ td [] [ input [ class "profileFirstNameInput", type_ "text", placeholder "first name", onInput FirstNameInput, value <| nameText profile.firstName ] [] ]
                    , td [] [ input [ class "profileNameInput", type_ "text", placeholder "last name", onInput LastNameInput, value <| nameText profile.lastName ] [] ]
                    ]
                ]
            , input [ class "profileInput", type_ "text", placeholder "email", onInput EmailInput, value <| emailText profile.email ] []
            , br [] []
            , table []
                [ tr []
                    [ td [] [ input [ class "profileTopicInput", type_ "text", placeholder "topic", onKeyDown KeyDown, onInput InputTopic, value (topicText model.currentTopic) ] [] ]
                    ]
                , tr [] [ td [] [ suggestionsUI (model.topicSuggestions |> List.map (\t -> topicText t)) ] ]
                , tr [] [ td [] [ div [] selectedTopicsUI ] ]
                ]
            , br [] []
            , button [ class "saveProfile", onClick Update ] [ text "Save" ]
            ]
