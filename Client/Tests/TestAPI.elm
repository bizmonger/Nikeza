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


someArticleTitle1 : Title
someArticleTitle1 =
    Title "Some Article Title 1"


someArticleTitle2 : Title
someArticleTitle2 =
    Title "Some Article Title 2"


someArticleTitle3 : Title
someArticleTitle3 =
    Title "Some Article Title 3"


someVideoTitle1 : Title
someVideoTitle1 =
    Title "Some Video Title 1"


someVideoTitle2 : Title
someVideoTitle2 =
    Title "Some Video Title 2"


someVideoTitle3 : Title
someVideoTitle3 =
    Title "Some Video Title 3"


somePodcastTitle1 : Title
somePodcastTitle1 =
    Title "Some Podcast Title 1"


somePodcastTitle2 : Title
somePodcastTitle2 =
    Title "Some Podcast Title 2"


somePodcastTitle3 : Title
somePodcastTitle3 =
    Title "Some Podcast Title 3"


someQuestionTitle1 : Title
someQuestionTitle1 =
    Title "Some Question Title 1"


someQuestionTitle2 : Title
someQuestionTitle2 =
    Title "Some Question Title 2"


someQuestionTitle3 : Title
someQuestionTitle3 =
    Title "Some Question Title 3"


someDescrtiption : String
someDescrtiption =
    "some description..."


profile1 : Profile
profile1 =
    Profile profileId1 (Name "Contributor 1") someImageUrl someDescrtiption [ someTopic1, someTopic2, someTopic3 ]


profile2 : Profile
profile2 =
    Profile profileId2 (Name "Contributor 2") someImageUrl someDescrtiption [ someTopic1, someTopic2, someTopic3 ]


profile3 : Profile
profile3 =
    Profile profileId3 (Name "Contributor 3") someImageUrl someDescrtiption [ someTopic1, someTopic2, someTopic3 ]


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


contributors : List Profile
contributors =
    [ profile1
    , profile2
    , profile3
    ]


links : ContentType -> Id -> List Link
links contentType profileId =
    case contentType of
        Article ->
            [ Link profile1 someArticleTitle1 someUrl [ someTopic1 ]
            , Link profile2 someArticleTitle2 someUrl [ someTopic2 ]
            , Link profile3 someArticleTitle3 someUrl [ someTopic3 ]
            ]

        Video ->
            [ Link profile1 someVideoTitle1 someUrl [ someTopic1 ]
            , Link profile2 someVideoTitle2 someUrl [ someTopic2 ]
            , Link profile3 someVideoTitle3 someUrl [ someTopic3 ]
            ]

        Podcast ->
            [ Link profile1 somePodcastTitle1 someUrl [ someTopic1 ]
            , Link profile2 somePodcastTitle2 someUrl [ someTopic2 ]
            , Link profile3 somePodcastTitle3 someUrl [ someTopic3 ]
            ]

        Answer ->
            [ Link profile1 someQuestionTitle1 someUrl [ someTopic1 ]
            , Link profile2 someQuestionTitle2 someUrl [ someTopic2 ]
            , Link profile3 someQuestionTitle3 someUrl [ someTopic3 ]
            ]

        All ->
            [ Link profile1 someArticleTitle1 someUrl [ someTopic1 ]
            , Link profile2 someArticleTitle2 someUrl [ someTopic2 ]
            , Link profile3 someArticleTitle3 someUrl [ someTopic3 ]
            ]


topics : Id -> List Topic
topics profileId =
    profileId
        |> links All
        |> List.map (\p -> p.topics)
        |> List.concat


contributor : Id -> Maybe Profile
contributor id =
    if id == profileId1 then
        Just profile1
    else if id == profileId2 then
        Just profile2
    else if id == profileId3 then
        Just profile3
    else
        Nothing


topicLinks : Topic -> ContentType -> Id -> List Link
topicLinks topic contentType id =
    id
        |> links contentType
        |> List.filter (\a -> a.topics |> List.member topic)


connections : Id -> List Connection
connections profileId =
    [ { platform = "WordPress", username = "bizmonger" }
    , { platform = "YouTube", username = "bizmonger" }
    , { platform = "StackOverflow", username = "scott-nimrod" }
    ]


usernameToId : String -> Id
usernameToId username =
    case username of
        "test" ->
            profileId1

        "profile_1" ->
            profileId1

        "profile_2" ->
            profileId2

        "profile_3" ->
            profileId3

        _ ->
            Id undefined


platforms : List Platform
platforms =
    [ Platform "WordPress"
    , Platform "YouTube"
    , Platform "Vimeo"
    , Platform "Medium"
    , Platform "StackOverflow"
    ]
