module Tests.TestAPI exposing (..)

import Controls.Login as Login exposing (Model)
import Domain.Core exposing (..)


profileId1 : Id
profileId1 =
    Id "profile_1"


profileId2 : Id
profileId2 =
    Id "profile_2"


profileId3 : Id
profileId3 =
    Id "profile_3"


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
    [ Topic "F#", Topic "Elm", Topic "Xamarin", Topic "WPF" ]


profile1 : Profile
profile1 =
    Profile profileId1 (Contributor "Contributor 1") someImageUrl someDescrtiption someTopics


profile2 : Profile
profile2 =
    Profile profileId2 (Contributor "Contributor 2") someImageUrl someDescrtiption someTopics


profile3 : Profile
profile3 =
    Profile profileId3 (Contributor "Contributor 3") someImageUrl someDescrtiption someTopics


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
    [ Podcast <| Post profile1 someTitle someUrl
    , Podcast <| Post profile2 someTitle someUrl
    , Podcast <| Post profile3 someTitle someUrl
    ]


recentArticles : List Article
recentArticles =
    [ Article <| Post profile1 someTitle someUrl
    , Article <| Post profile2 someTitle someUrl
    , Article <| Post profile3 someTitle someUrl
    ]


recentVideos : List Video
recentVideos =
    [ Video <| Post profile1 someTitle someUrl
    , Video <| Post profile2 someTitle someUrl
    , Video <| Post profile3 someTitle someUrl
    ]


articles : Id -> List Article
articles profileId =
    [ Article <| Post profile1 someTitle someUrl
    , Article <| Post profile2 someTitle someUrl
    , Article <| Post profile3 someTitle someUrl
    ]


videos : Id -> List Video
videos profileId =
    [ Video <| Post profile1 someTitle someUrl
    , Video <| Post profile2 someTitle someUrl
    , Video <| Post profile3 someTitle someUrl
    ]


podcasts : Id -> List Podcast
podcasts profileId =
    [ Podcast <| Post profile1 someTitle someUrl
    , Podcast <| Post profile2 someTitle someUrl
    , Podcast <| Post profile3 someTitle someUrl
    ]


latestPosts : Id -> ContentType -> List Post
latestPosts id contentType =
    []


contributorUrl : Id -> Url
contributorUrl id =
    Url <| "/#/contributor/" ++ getId id


getContributor : Id -> Maybe Profile
getContributor id =
    if id == profileId1 then
        Just profile1
    else if id == profileId2 then
        Just profile2
    else if id == profileId3 then
        Just profile3
    else
        Nothing
