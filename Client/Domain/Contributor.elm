module Domain.Contributor exposing (..)

import Domain.Core exposing (..)
import Controls.ProfileThumbnail as ProfileThumbnail exposing (..)
import Settings exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)


main =
    Html.beginnerProgram
        { model = model
        , update = update
        , view = view
        }



-- MODEL


type alias Model =
    { profile : Profile
    , topics : List Topic
    , articles : List Post
    , videos : List Post
    , podcasts : List Post
    }


model : Model
model =
    { profile = Profile (Id undefined) (Contributor undefined) (Url undefined) undefined []
    , topics = []
    , articles = []
    , videos = []
    , podcasts = []
    }


type Msg
    = TopicSelected
    | ArticlesSelected
    | VideosSelected
    | PodcastsSelected
    | None ProfileThumbnail.Msg


update : Msg -> Model -> Model
update msg model =
    case msg of
        TopicSelected ->
            model

        ArticlesSelected ->
            { model | articles = runtime.latestPosts model.profile.id Article }

        VideosSelected ->
            { model | videos = runtime.latestPosts model.profile.id Video }

        PodcastsSelected ->
            { model | podcasts = runtime.latestPosts model.profile.id Podcast }

        None subMsg ->
            model



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
                            , div [] <| contentUI (model.profile.id |> runtime.posts Video)
                            , tr [] [ td [] [ b [] [ text "Podcasts" ] ] ]
                            , div [] <| contentUI (model.profile.id |> runtime.posts Podcast)
                            , tr [] [ td [] [ b [] [ text "Articles" ] ] ]
                            , div [] <| contentUI (model.profile.id |> runtime.posts Article)
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
        [ input [ type_ "checkbox", name "topic", onClick TopicSelected, value <| getTopic topic ] []
        , label [] [ text <| getTopic topic ]
        ]


topicsUI : List Topic -> Html Msg
topicsUI topics =
    let
        formattedTopics =
            topics |> List.map topicTocheckbox
    in
        Html.form [ action "" ] formattedTopics
