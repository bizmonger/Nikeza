module Domain.Contributor exposing (..)

import Domain.Core exposing (..)
import Settings exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)


-- MODEL


type alias Model =
    { profile : Profile
    , topics : List Topic
    , articles : List Post
    , videos : List Post
    , podcasts : List Post
    }


type Msg
    = TopicSelected Topic


update : Msg -> Model -> Model
update msg model =
    case msg of
        TopicSelected topic ->
            { model
                | articles = model.profile.id |> runtime.posts Article
                , podcasts = model.profile.id |> runtime.posts Podcast
                , videos = model.profile.id |> runtime.posts Video
            }



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ table []
            [ tr []
                [ table []
                    [ tr []
                        [ td [] [ img [ src <| getUrl <| model.profile.imageUrl, width 100, height 100 ] [] ]
                        , td []
                            [ topicsUI model.profile.topics ]
                        , table []
                            [ tr [] [ td [] [ b [] [ text "Videos" ] ] ]
                            , div [] <| contentUI model.videos
                            , tr [] [ td [] [ b [] [ text "Podcasts" ] ] ]
                            , div [] <| contentUI model.podcasts
                            , tr [] [ td [] [ b [] [ text "Articles" ] ] ]
                            , div [] <| contentUI model.articles
                            ]
                        ]
                    , tr [] [ td [] [ text <| getName model.profile.name ] ]
                    , tr [] [ td [] [ p [] [ text model.profile.bio ] ] ]
                    ]
                ]
            ]
        ]


contentUI : List Post -> List (Html Msg)
contentUI videos =
    videos |> List.map (\post -> a [ href <| getUrl post.url ] [ text <| getTitle post.title, br [] [] ])


topicTocheckbox : Topic -> Html Msg
topicTocheckbox topic =
    div []
        [ input [ type_ "checkbox", name "topic", onClick <| TopicSelected topic, value <| getTopic topic ] []
        , label [] [ text <| getTopic topic ]
        ]


topicsUI : List Topic -> Html Msg
topicsUI topics =
    let
        formattedTopics =
            topics |> List.map topicTocheckbox
    in
        Html.form [ action "" ] formattedTopics
