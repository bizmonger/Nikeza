module Tests.TestAPI exposing (..)

import Controls.Login as Login exposing (Model)
import Domain.Core exposing (..)


someUrl : Url
someUrl =
    Url "http://some_url.com"


someTitle : Title
someTitle =
    Title "Some Title"


submitter1 : Submitter
submitter1 =
    Submitter "Submitter 1"


submitter2 : Submitter
submitter2 =
    Submitter "Submitter 2"


submitter3 : Submitter
submitter3 =
    Submitter "Submitter 3"


tryLogin : Login.Model -> Login.Model
tryLogin credentials =
    let
        successful =
            String.toLower credentials.username == "test" && String.toLower credentials.password == "test"
    in
        if successful then
            { username = credentials.username, password = credentials.password, loggedIn = True }
        else
            { username = credentials.username, password = credentials.password, loggedIn = False }


recentSubmitters : List Submitter
recentSubmitters =
    [ submitter1
    , submitter2
    , submitter3
    ]


recentPodcasts : List Podcast
recentPodcasts =
    [ Podcast (Post submitter1 someTitle someUrl) ]


recentArticles : List Article
recentArticles =
    [ Article (Post submitter2 someTitle someUrl) ]


recentVideos : List Video
recentVideos =
    [ Video (Post submitter3 someTitle someUrl) ]
