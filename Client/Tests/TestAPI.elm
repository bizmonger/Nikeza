module Tests.TestAPI exposing (..)

import Controls.Login as Login exposing (Model)
import Domain.Core exposing (..)
import List.Extra as ListHelper exposing (..)


profileId1 : Id
profileId1 =
    Id "profile_1"


profileId2 : Id
profileId2 =
    Id "profile_2"


profileId3 : Id
profileId3 =
    Id "profile_3"


someTopic1 : Topic
someTopic1 =
    Topic "Topic-1"


someTopic2 : Topic
someTopic2 =
    Topic "Topic-2"


someTopic3 : Topic
someTopic3 =
    Topic "Topic-3"


someUrl : Url
someUrl =
    Url "http://some_url.com"


someImageUrl : Url
someImageUrl =
    Url "http://www.ngu.edu/myimages/silhouette2230.jpg"


someArticleTitle : Title
someArticleTitle =
    Title "Some Article Title"


someVideoTitle : Title
someVideoTitle =
    Title "Some Video Title"


somePodcastTitle : Title
somePodcastTitle =
    Title "Some Podcast Title"


someDescrtiption : String
someDescrtiption =
    "some description..."


profile1 : Profile
profile1 =
    Profile profileId1 (Contributor "Contributor 1") someImageUrl someDescrtiption [ someTopic1, someTopic2, someTopic3 ]


profile2 : Profile
profile2 =
    Profile profileId2 (Contributor "Contributor 2") someImageUrl someDescrtiption [ someTopic1, someTopic2, someTopic3 ]


profile3 : Profile
profile3 =
    Profile profileId3 (Contributor "Contributor 3") someImageUrl someDescrtiption [ someTopic1, someTopic2, someTopic3 ]


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


posts : ContentType -> Id -> List Post
posts contentType profileId =
    case contentType of
        Article ->
            [ Post profile1 someArticleTitle someUrl [ someTopic1 ]
            , Post profile2 someArticleTitle someUrl [ someTopic2 ]
            , Post profile3 someArticleTitle someUrl [ someTopic3 ]
            ]

        Video ->
            [ Post profile1 someVideoTitle someUrl [ someTopic1 ]
            , Post profile2 someVideoTitle someUrl [ someTopic2 ]
            , Post profile3 someVideoTitle someUrl [ someTopic3 ]
            ]

        Podcast ->
            [ Post profile1 somePodcastTitle someUrl [ someTopic1 ]
            , Post profile2 somePodcastTitle someUrl [ someTopic2 ]
            , Post profile3 somePodcastTitle someUrl [ someTopic3 ]
            ]

        All ->
            [ Post profile1 someArticleTitle someUrl [ someTopic1 ]
            , Post profile2 someArticleTitle someUrl [ someTopic2 ]
            , Post profile3 someArticleTitle someUrl [ someTopic3 ]
            ]


topics : Id -> List Topic
topics profileId =
    profileId
        |> posts All
        |> List.map (\p -> p.topics)
        |> List.concat


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
