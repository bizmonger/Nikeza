module Tests.TestAPI exposing (..)

import Controls.Login as Login exposing (Model)
import Domain.Core exposing (..)


someId : Id
someId =
    Id "some_id"


someUrl : Url
someUrl =
    Url "http://some_url.com"


someImageUrl : Url
someImageUrl =
    Url "http://www.ngu.edu/myimages/silhouette2230.jpg"


someTitle : Title
someTitle =
    Title "Some Title"


someDescrtiption : String
someDescrtiption =
    "some description..."


someTags : List Tag
someTags =
    [ Tag "F#", Tag "Elm", Tag "Test Automation", Tag "Xamarin", Tag "WPF" ]


submitter1 : Profile
submitter1 =
    Profile someId (Submitter "Submitter 1") someImageUrl someDescrtiption someTags


submitter2 : Profile
submitter2 =
    Profile someId (Submitter "Submitter 2") someImageUrl someDescrtiption someTags


submitter3 : Profile
submitter3 =
    Profile someId (Submitter "Submitter 3") someImageUrl someDescrtiption someTags


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


tagUrl : Id -> Tag -> Url
tagUrl id tag =
    someUrl


recentSubmitters : List Profile
recentSubmitters =
    [ submitter1
    , submitter2
    , submitter3
    ]


recentPodcasts : List Podcast
recentPodcasts =
    [ Podcast <| Post submitter1 someTitle someImageUrl
    , Podcast <| Post submitter2 someTitle someImageUrl
    , Podcast <| Post submitter3 someTitle someImageUrl
    ]


recentArticles : List Article
recentArticles =
    [ Article <| Post submitter1 someTitle someImageUrl
    , Article <| Post submitter2 someTitle someImageUrl
    , Article <| Post submitter3 someTitle someImageUrl
    ]


recentVideos : List Video
recentVideos =
    [ Video <| Post submitter1 someTitle someImageUrl
    , Video <| Post submitter2 someTitle someImageUrl
    , Video <| Post submitter3 someTitle someImageUrl
    ]
