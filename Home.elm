module Home exposing (..)

import Domain.Core exposing (..)
import Controls.Login as Login exposing (..)
import Settings exposing (..)
import Tests.TestAPI as TestAPI exposing (tryLogin)
import Services.Server as Services exposing (tryLogin)
import Html exposing (..)
import Html.Attributes exposing (..)


-- elm-live Home.elm --open --output=home.js


main =
    Html.beginnerProgram
        { model = model
        , update = update
        , view = view
        }


type alias Dependencies =
    { tryLogin : Loginfunction
    , topicUrl : TopicUrlFunction
    , latestPosts : LatestPostsfunction
    }


runtime : Dependencies
runtime =
    case configuration of
        Integration ->
            Dependencies Services.tryLogin Services.topicUrl Services.latestPosts

        Isolation ->
            Dependencies TestAPI.tryLogin TestAPI.topicUrl TestAPI.latestPosts



-- MODEL


type alias Content =
    { videos : List Video
    , articles : List Article
    , podcasts : List Podcast
    }


type alias Model =
    { content : Content
    , contributors : List Contributor
    , login : Login.Model
    }


model : Model
model =
    { content = Content [] [] []
    , contributors = []
    , login = Login.model
    }



-- UPDATE


type Msg
    = Video Video
    | Article Article
    | Contributor Contributor
    | Search String
    | Register
    | OnLogin Login.Msg


update : Msg -> Model -> Model
update msg model =
    case msg of
        Video v ->
            model

        Article v ->
            model

        Contributor v ->
            model

        Search v ->
            model

        Register ->
            model

        OnLogin subMsg ->
            case subMsg of
                Login.Attempt v ->
                    let
                        latest =
                            Login.update subMsg model.login
                    in
                        { model | login = runtime.tryLogin latest }

                Login.UserInput _ ->
                    { model | login = Login.update subMsg model.login }

                Login.PasswordInput _ ->
                    { model | login = Login.update subMsg model.login }



-- VIEW


view : Model -> Html Msg
view model =
    div []
        [ header []
            [ label [] [ text "Nikeza" ]
            , model |> sessionUI
            ]
        , div [] contributors
        , footer [ class "copyright" ]
            [ label [] [ text "(c)2017" ]
            , a [ href "" ] [ text "GitHub" ]
            ]
        ]


contributors : List (Html Msg)
contributors =
    TestAPI.recentContributors |> List.map thumbnail


thumbnail : Profile -> Html Msg
thumbnail profile =
    let
        formatTopic topic =
            a [ href <| getUrl <| topicUrl runtime.topicUrl profile.id topic ] [ i [] [ text <| gettopic topic ] ]

        concatTopics topic1 topic2 =
            span []
                [ topic1
                , label [] [ text " " ]
                , topic2
                , label [] [ text " " ]
                ]

        topics =
            List.foldr concatTopics (div [] []) (profile.topics |> List.map formatTopic)

        topicsAndBio =
            div []
                [ topics
                , br [] []
                , label [] [ text profile.bio ]
                ]
    in
        div []
            [ table []
                [ tr []
                    [ td [] [ img [ src <| getUrl profile.imageUrl, width 50, height 50 ] [] ]
                    , td [] [ topicsAndBio ]
                    ]
                ]
            , label [] [ text (profile.name |> getName) ]
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
            Html.map OnLogin <| Login.view model.login
        else
            div [ class "signin" ] [ welcome, signout ]
