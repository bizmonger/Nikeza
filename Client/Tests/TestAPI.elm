module Tests.TestAPI exposing (..)

import Controls.Login as Login exposing (Model)
import Controls.Register as Register exposing (Model)
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
    Topic "WPF" True


someTopic2 : Topic
someTopic2 =
    Topic "Xamarin.Forms" True


someTopic3 : Topic
someTopic3 =
    Topic "F#" True


someTopic4 : Topic
someTopic4 =
    Topic "Elm" True


someTopic5 : Topic
someTopic5 =
    Topic "unit-tests" False


someUrl : Url
someUrl =
    Url "http://some_url.com"


someImageUrl : Url
someImageUrl =
    Url "http://www.ngu.edu/myimages/silhouette2230.jpg"


contentProvider1Links : Links
contentProvider1Links =
    Links (answers profileId1) (articles profileId1) (videos profileId1) (podcasts profileId1)


contentProvider2Links : Links
contentProvider2Links =
    Links (answers profileId2) (articles profileId2) (videos profileId2) (podcasts profileId2)


contentProvider3Links : Links
contentProvider3Links =
    Links (answers profileId3) (articles profileId3) (videos profileId3) (podcasts profileId3)


someArticleTitle1 : Title
someArticleTitle1 =
    Title "Some WPF Article"


someArticleTitle2 : Title
someArticleTitle2 =
    Title "Some Xamarin.Forms Article"


someArticleTitle3 : Title
someArticleTitle3 =
    Title "Some F# Article"


someArticleTitle4 : Title
someArticleTitle4 =
    Title "Some Elm Article"


someArticleTitle5 : Title
someArticleTitle5 =
    Title "Some Unit Test Article"


someVideoTitle1 : Title
someVideoTitle1 =
    Title "Some WPF Video"


someVideoTitle2 : Title
someVideoTitle2 =
    Title "Some Xaarin.Forms Video"


someVideoTitle3 : Title
someVideoTitle3 =
    Title "Some F# Video"


someVideoTitle4 : Title
someVideoTitle4 =
    Title "Some Elm Video"


someVideoTitle5 : Title
someVideoTitle5 =
    Title "Some Unit Test Video"


somePodcastTitle1 : Title
somePodcastTitle1 =
    Title "Some WPF Podcast"


somePodcastTitle2 : Title
somePodcastTitle2 =
    Title "Some Xamarin.Forms Podcast"


somePodcastTitle3 : Title
somePodcastTitle3 =
    Title "Some F# Podcast"


somePodcastTitle4 : Title
somePodcastTitle4 =
    Title "Some Elm Podcast"


somePodcastTitle5 : Title
somePodcastTitle5 =
    Title "Some Unit Test Podcast"


someQuestionTitle1 : Title
someQuestionTitle1 =
    Title "Some WPF Question"


someQuestionTitle2 : Title
someQuestionTitle2 =
    Title "Some Xamarin.Forms Question"


someQuestionTitle3 : Title
someQuestionTitle3 =
    Title "Some F# Question"


someQuestionTitle4 : Title
someQuestionTitle4 =
    Title "Some Elm Question"


someQuestionTitle5 : Title
someQuestionTitle5 =
    Title "Some Unit Test Question"


someDescrtiption : String
someDescrtiption =
    "some description..."


someEmail : Email
someEmail =
    Email "abc@abc.com"


profile1 : Profile
profile1 =
    Profile profileId1 (Name "ContentProvider 1") someEmail someImageUrl someDescrtiption (profileId1 |> connections)


profile2 : Profile
profile2 =
    Profile profileId2 (Name "ContentProvider 2") someEmail someImageUrl someDescrtiption (profileId2 |> connections)


profile3 : Profile
profile3 =
    Profile profileId3 (Name "ContentProvider 3") someEmail someImageUrl someDescrtiption (profileId3 |> connections)


contentProvider1 : ContentProvider
contentProvider1 =
    ContentProvider profile1 True topics contentProvider1Links


contentProvider2 : ContentProvider
contentProvider2 =
    ContentProvider profile2 True topics contentProvider2Links


contentProvider3 : ContentProvider
contentProvider3 =
    ContentProvider profile3 True topics contentProvider3Links



-- FUNCTIONS


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


tryRegister : Register.Model -> Result String ContentProvider
tryRegister form =
    let
        successful =
            form.password == form.confirm
    in
        if successful then
            let
                profile =
                    Profile (Id undefined) (Name form.name) (Email form.email) someImageUrl "" []
            in
                Ok <| ContentProvider profile True [] initLinks
        else
            Err "Registration failed"


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


addLink : Id -> Link -> Result String Links
addLink profileId link =
    let
        currentLinks =
            profileId |> links
    in
        case link.contentType of
            All ->
                Err "Failed to add link: Cannot add link to 'ALL'"

            Unknown ->
                Err "Failed to add link: Contenttype of link is unknown"

            Answer ->
                Ok { currentLinks | answers = link :: currentLinks.answers }

            Article ->
                Ok { currentLinks | articles = link :: currentLinks.articles }

            Video ->
                Ok { currentLinks | videos = link :: currentLinks.videos }

            podcast ->
                Ok { currentLinks | podcasts = link :: currentLinks.podcasts }


removeLink : Id -> Link -> Result String Links
removeLink profileId link =
    let
        currentLinks =
            profileId |> links
    in
        case link.contentType of
            All ->
                Err "Failed to add link: Cannot add link to 'ALL'"

            Unknown ->
                Err "Failed to add link: Contenttype of link is unknown"

            Answer ->
                let
                    updated =
                        currentLinks.answers |> List.filter (\link -> currentLinks.answers |> List.member link)
                in
                    Ok { currentLinks | answers = updated }

            Article ->
                let
                    updated =
                        currentLinks.articles |> List.filter (\link -> currentLinks.articles |> List.member link)
                in
                    Ok { currentLinks | articles = updated }

            Video ->
                let
                    updated =
                        currentLinks.videos |> List.filter (\link -> currentLinks.videos |> List.member link)
                in
                    Ok { currentLinks | videos = updated }

            podcast ->
                let
                    updated =
                        currentLinks.podcasts |> List.filter (\link -> currentLinks.podcasts |> List.member link)
                in
                    Ok { currentLinks | podcasts = updated }


linksToContent : ContentType -> Id -> List Link
linksToContent contentType profileId =
    -- NOTE !!! We're hardcoding a profile here due to some unresolved bug
    case contentType of
        Article ->
            [ Link profile1 someArticleTitle1 someUrl Article [ someTopic1 ]
            , Link profile1 someArticleTitle2 someUrl Article [ someTopic2 ]
            , Link profile1 someArticleTitle3 someUrl Article [ someTopic3 ]
            , Link profile1 someArticleTitle4 someUrl Article [ someTopic4 ]
            , Link profile1 someArticleTitle5 someUrl Article [ someTopic5 ]
            ]

        Video ->
            [ Link profile1 someVideoTitle1 someUrl Video [ someTopic1 ]
            , Link profile1 someVideoTitle2 someUrl Video [ someTopic2 ]
            , Link profile1 someVideoTitle3 someUrl Video [ someTopic3 ]
            , Link profile1 someVideoTitle4 someUrl Video [ someTopic4 ]
            , Link profile1 someVideoTitle5 someUrl Video [ someTopic5 ]
            ]

        Podcast ->
            [ Link profile1 somePodcastTitle1 someUrl Podcast [ someTopic1 ]
            , Link profile1 somePodcastTitle2 someUrl Podcast [ someTopic2 ]
            , Link profile1 somePodcastTitle3 someUrl Podcast [ someTopic3 ]
            , Link profile1 somePodcastTitle4 someUrl Podcast [ someTopic4 ]
            , Link profile1 somePodcastTitle5 someUrl Podcast [ someTopic5 ]
            ]

        Answer ->
            [ Link profile1 someQuestionTitle1 someUrl Answer [ someTopic1 ]
            , Link profile1 someQuestionTitle2 someUrl Answer [ someTopic2 ]
            , Link profile1 someQuestionTitle3 someUrl Answer [ someTopic3 ]
            , Link profile1 someQuestionTitle4 someUrl Answer [ someTopic4 ]
            , Link profile1 someQuestionTitle5 someUrl Answer [ someTopic5 ]
            ]

        All ->
            []

        Unknown ->
            []


suggestedTopics : String -> List Topic
suggestedTopics search =
    if not <| isEmpty search then
        topics |> List.filter (\t -> (getTopic t) |> toLower |> contains (search |> toLower))
    else
        []


contentProvider : Id -> Maybe ContentProvider
contentProvider id =
    if id == profileId1 then
        Just contentProvider1
    else if id == profileId2 then
        Just contentProvider2
    else if id == profileId3 then
        Just contentProvider3
    else
        Nothing


contentProviders : List ContentProvider
contentProviders =
    [ contentProvider1
    , contentProvider2
    , contentProvider3
    ]


topics : List Topic
topics =
    [ someTopic1, someTopic2, someTopic3, someTopic4, someTopic5 ]


topicLinks : Topic -> ContentType -> Id -> List Link
topicLinks topic contentType id =
    id
        |> linksToContent contentType
        |> List.filter (\l -> l.topics |> List.member topic)


connections : Id -> List Source
connections profileId =
    [ { platform = "WordPress", username = "bizmonger" }
    , { platform = "YouTube", username = "bizmonger" }
    , { platform = "StackOverflow", username = "scott-nimrod" }
    ]


addSource : Id -> Source -> Result String (List Source)
addSource profileId connection =
    Ok <| connection :: (profileId |> connections)


removeSource : Id -> Source -> Result String (List Source)
removeSource profileId connection =
    Ok (profileId |> connections |> List.filter (\c -> profileId |> connections |> List.member connection))


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
