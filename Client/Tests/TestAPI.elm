module Tests.TestAPI exposing (..)

import Controls.Login as Login exposing (Model)
import Domain.Core as Domain exposing (..)
import String exposing (..)


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


contributor1Links : Links
contributor1Links =
    Links (answers profileId1) (articles profileId1) (videos profileId1) (podcasts profileId1)


contributor2Links : Links
contributor2Links =
    Links (answers profileId2) (articles profileId2) (videos profileId2) (podcasts profileId2)


contributor3Links : Links
contributor3Links =
    Links (answers profileId3) (articles profileId3) (videos profileId3) (podcasts profileId3)


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
    Profile profileId1 (Name "Contributor 1") someImageUrl someDescrtiption (profileId1 |> connections) [ someTopic1, someTopic2, someTopic3 ]


profile2 : Profile
profile2 =
    Profile profileId2 (Name "Contributor 2") someImageUrl someDescrtiption (profileId2 |> connections) [ someTopic1, someTopic2, someTopic3 ]


profile3 : Profile
profile3 =
    Profile profileId3 (Name "Contributor 3") someImageUrl someDescrtiption (profileId3 |> connections) [ someTopic1, someTopic2, someTopic3 ]


contributor1 : Contributor
contributor1 =
    Contributor profile1 initConnection initNewLinks True topics contributor1Links


contributor2 : Contributor
contributor2 =
    Contributor profile2 initConnection initNewLinks True topics contributor2Links


contributor3 : Contributor
contributor3 =
    Contributor profile3 initConnection initNewLinks True topics contributor3Links


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


answers : Id -> List Link
answers id =
    id |> linksToContent Answer


articles : Id -> List Link
articles id =
    id |> linksToContent Article


videos : Id -> List Link
videos id =
    id |> linksToContent Video


podcasts : Id -> List Link
podcasts id =
    id |> linksToContent Podcast


links : Id -> Links
links id =
    { answers = id |> linksToContent Answer
    , articles = id |> linksToContent Article
    , videos = id |> linksToContent Video
    , podcasts = id |> linksToContent Podcast
    }


linksToContent : ContentType -> Id -> List Link
linksToContent contentType profileId =
    case contentType of
        Article ->
            [ Link profile1 someArticleTitle1 someUrl Article [ someTopic1 ]
            , Link profile2 someArticleTitle2 someUrl Article [ someTopic2 ]
            , Link profile3 someArticleTitle3 someUrl Article [ someTopic3 ]
            ]

        Video ->
            [ Link profile1 someVideoTitle1 someUrl Video [ someTopic1 ]
            , Link profile2 someVideoTitle2 someUrl Video [ someTopic2 ]
            , Link profile3 someVideoTitle3 someUrl Video [ someTopic3 ]
            ]

        Podcast ->
            [ Link profile1 somePodcastTitle1 someUrl Podcast [ someTopic1 ]
            , Link profile2 somePodcastTitle2 someUrl Podcast [ someTopic2 ]
            , Link profile3 somePodcastTitle3 someUrl Podcast [ someTopic3 ]
            ]

        Answer ->
            [ Link profile1 someQuestionTitle1 someUrl Answer [ someTopic1 ]
            , Link profile2 someQuestionTitle2 someUrl Answer [ someTopic2 ]
            , Link profile3 someQuestionTitle3 someUrl Answer [ someTopic3 ]
            ]

        All ->
            [ Link profile1 someArticleTitle1 someUrl Article [ someTopic1 ]
            , Link profile2 someArticleTitle2 someUrl Article [ someTopic2 ]
            , Link profile3 someArticleTitle3 someUrl Article [ someTopic3 ]
            ]

        Unknown ->
            []


suggestedTopics : String -> List Topic
suggestedTopics search =
    if not <| isEmpty search then
        topics |> List.filter (\t -> (getTopic t) |> toLower |> contains (search |> toLower))
    else
        []


contributor : Id -> Maybe Contributor
contributor id =
    if id == profileId1 then
        Just contributor1
    else if id == profileId2 then
        Just contributor2
    else if id == profileId3 then
        Just contributor3
    else
        Nothing


contributors : List Contributor
contributors =
    [ contributor1
    , contributor2
    , contributor3
    ]


topics : List Topic
topics =
    [ someTopic1, someTopic2, someTopic3 ]


topicLinks : Topic -> ContentType -> Id -> List Link
topicLinks topic contentType id =
    id
        |> linksToContent contentType
        |> List.filter (\l -> l.topics |> List.member topic)


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
