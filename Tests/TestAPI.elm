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


someTopics : List Topic
someTopics =
    [ Topic "F#", Topic "Elm", Topic "Test Automation", Topic "Xamarin", Topic "WPF" ]


profile1 : Profile
profile1 =
    Profile someId (Contributor "Contributor 1") someImageUrl someDescrtiption someTopics


profile2 : Profile
profile2 =
    Profile someId (Contributor "Contributor 2") someImageUrl someDescrtiption someTopics


profile3 : Profile
profile3 =
    Profile someId (Contributor "Contributor 3") someImageUrl someDescrtiption someTopics


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


topicUrl : Id -> Topic -> Url
topicUrl id topic =
    someUrl


recentContributors : List Profile
recentContributors =
    [ profile1
    , profile2
    , profile3
    ]


recentPodcasts : List Podcast
recentPodcasts =
    [ Podcast <| Post profile1 someTitle someImageUrl
    , Podcast <| Post profile2 someTitle someImageUrl
    , Podcast <| Post profile3 someTitle someImageUrl
    ]


recentArticles : List Article
recentArticles =
    [ Article <| Post profile1 someTitle someImageUrl
    , Article <| Post profile2 someTitle someImageUrl
    , Article <| Post profile3 someTitle someImageUrl
    ]


recentVideos : List Video
recentVideos =
    [ Video <| Post profile1 someTitle someImageUrl
    , Video <| Post profile2 someTitle someImageUrl
    , Video <| Post profile3 someTitle someImageUrl
    ]


latestPosts : Id -> ContentType -> List Post
latestPosts id contentType =
    []


contributorUrl : Id -> Url
contributorUrl id =
    someUrl
