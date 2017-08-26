module Tests.TestAPI exposing (..)

import Domain.Core as Domain exposing (..)
import String exposing (..)
import Http exposing (..)
import Task exposing (succeed, perform)


profileId1 : Id
profileId1 =
    Id "profile_1"


profileId2 : Id
profileId2 =
    Id "profile_2"


profileId3 : Id
profileId3 =
    Id "profile_3"


profileId4 : Id
profileId4 =
    Id "profile_4"


profileId5 : Id
profileId5 =
    Id "profile_5"


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


profile1ImageUrl : Url
profile1ImageUrl =
    Url "Assets/ProfileImages/Bizmonger.png"


profile2ImageUrl : Url
profile2ImageUrl =
    Url "Assets/ProfileImages/Pablo.jpg"


profile3ImageUrl : Url
profile3ImageUrl =
    Url "Assets/ProfileImages/Adam.jpg"


profile4ImageUrl : Url
profile4ImageUrl =
    Url "Assets/ProfileImages/Mitch.jpg"


profile5ImageUrl : Url
profile5ImageUrl =
    Url "Assets/ProfileImages/Ody.jpg"


provider1Links : Links
provider1Links =
    Links (answers profileId1) (articles profileId1) (videos profileId1) (podcasts profileId1)


provider2Links : Links
provider2Links =
    Links (answers profileId2) (articles profileId2) (videos profileId2) (podcasts profileId2)


provider3Links : Links
provider3Links =
    Links (answers profileId3) (articles profileId3) (videos profileId3) (podcasts profileId3)


provider4Links : Links
provider4Links =
    Links (answers profileId4) (articles profileId4) (videos profileId4) (podcasts profileId4)


provider5Links : Links
provider5Links =
    Links (answers profileId5) (articles profileId5) (videos profileId5) (podcasts profileId5)


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


someArticleTitle6 : Title
someArticleTitle6 =
    Title "Some Docker Article"


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


someVideoTitle6 : Title
someVideoTitle6 =
    Title "Some .Net Core Video"


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


somePodcastTitle6 : Title
somePodcastTitle6 =
    Title "Some .Net Core Podcast"


someAnswerTitle1 : Title
someAnswerTitle1 =
    Title "Some WPF Answer"


someAnswerTitle2 : Title
someAnswerTitle2 =
    Title "Some Xamarin.Forms Answer"


someAnswerTitle3 : Title
someAnswerTitle3 =
    Title "Some F# Answer"


someAnswerTitle4 : Title
someAnswerTitle4 =
    Title "Some Elm Answer"


someAnswerTitle5 : Title
someAnswerTitle5 =
    Title "Some Unit Test Answer"


someAnswerTitle6 : Title
someAnswerTitle6 =
    Title "Some Property-based Testing Answer"


someDescrtiption : String
someDescrtiption =
    "some description..."


someEmail : Email
someEmail =
    Email "abc@abc.com"


profile1 : Profile
profile1 =
    Profile profileId1 (Name "Scott") (Name "Nimrod") someEmail profile1ImageUrl someDescrtiption (profileId1 |> sources)


profile2 : Profile
profile2 =
    Profile profileId2 (Name "Pablo") (Name "Rivera") someEmail profile2ImageUrl someDescrtiption (profileId2 |> sources)


profile3 : Profile
profile3 =
    Profile profileId3 (Name "Adam") (Name "Wright") someEmail profile3ImageUrl someDescrtiption (profileId3 |> sources)


profile4 : Profile
profile4 =
    Profile profileId4 (Name "Mitchell") (Name "Tilbrook") someEmail profile4ImageUrl someDescrtiption (profileId4 |> sources)


profile5 : Profile
profile5 =
    Profile profileId5 (Name "Ody") (Name "Mbegbu") someEmail profile5ImageUrl someDescrtiption (profileId5 |> sources)


subscriptions : Id -> Subscribers
subscriptions profileId =
    if profileId == profileId1 then
        -- Weird error when we have provider3
        Subscribers [ provider2 ]
    else
        Subscribers []


followers : Id -> Subscribers
followers profileId =
    if profileId == profileId1 then
        Subscribers [ provider4, provider5 ]
    else if profileId == profileId2 then
        Subscribers [ provider1B, provider5 ]
    else if profileId == profileId3 then
        Subscribers [ provider4, provider5 ]
    else
        Subscribers []


recentLinks1 : List Link
recentLinks1 =
    [ Link profile1 someVideoTitle6 someUrl Video [ someTopic1 ] True
    ]


recentLinks2 : List Link
recentLinks2 =
    [ Link profile1 somePodcastTitle6 someUrl Video [ someTopic1 ] True
    , Link profile1 someAnswerTitle6 someUrl Video [ someTopic1 ] True
    ]


recentLinks3 : List Link
recentLinks3 =
    [ Link profile1 someArticleTitle6 someUrl Video [ someTopic1 ] True
    ]


provider1 : Provider
provider1 =
    Provider profile1 topics provider1Links recentLinks1 subscriptions followers


provider1B : Provider
provider1B =
    Provider profile1 topics provider1Links recentLinks1 subscriptions followers


provider2 : Provider
provider2 =
    Provider profile2 topics provider2Links recentLinks2 subscriptions followers


provider3 : Provider
provider3 =
    Provider profile3 topics provider3Links recentLinks3 subscriptions followers


provider4 : Provider
provider4 =
    Provider profile4 topics provider4Links [] subscriptions followers


provider5 : Provider
provider5 =
    Provider profile5 topics provider5Links [] subscriptions followers



-- FUNCTIONS


tryLogin : Credentials -> (Result Http.Error JsonProvider -> msg) -> Cmd msg
tryLogin credentials msg =
    let
        successful =
            String.toLower credentials.email == "test" && String.toLower credentials.password == "test"
    in
        if successful then
            JsonProvider initProfile initTopics initLinks []
                |> Result.Ok
                |> msg
                |> Task.succeed
                |> Task.perform identity
        else
            Cmd.none


tryRegister : Form -> (Result Http.Error JsonProfile -> msg) -> Cmd msg
tryRegister form msg =
    if form.password == form.confirm then
        JsonProfile (getId profileId1) form.firstName form.lastName form.email
            |> Result.Ok
            |> msg
            |> Task.succeed
            |> Task.perform identity
    else
        Cmd.none


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
            [ Link profile1 someArticleTitle1 someUrl Article [ someTopic1 ] False
            , Link profile1 someArticleTitle2 someUrl Article [ someTopic2 ] True
            , Link profile1 someArticleTitle3 someUrl Article [ someTopic3 ] False
            , Link profile1 someArticleTitle4 someUrl Article [ someTopic4 ] True
            , Link profile1 someArticleTitle5 someUrl Article [ someTopic5 ] False
            ]

        Video ->
            [ Link profile1 someVideoTitle1 someUrl Video [ someTopic1 ] False
            , Link profile1 someVideoTitle2 someUrl Video [ someTopic2 ] True
            , Link profile1 someVideoTitle3 someUrl Video [ someTopic3 ] False
            , Link profile1 someVideoTitle4 someUrl Video [ someTopic4 ] True
            , Link profile1 someVideoTitle5 someUrl Video [ someTopic5 ] False
            ]

        Podcast ->
            [ Link profile1 somePodcastTitle1 someUrl Podcast [ someTopic1 ] False
            , Link profile1 somePodcastTitle2 someUrl Podcast [ someTopic2 ] True
            , Link profile1 somePodcastTitle3 someUrl Podcast [ someTopic3 ] False
            , Link profile1 somePodcastTitle4 someUrl Podcast [ someTopic4 ] True
            , Link profile1 somePodcastTitle5 someUrl Podcast [ someTopic5 ] False
            ]

        Answer ->
            [ Link profile1 someAnswerTitle1 someUrl Answer [ someTopic1 ] False
            , Link profile1 someAnswerTitle2 someUrl Answer [ someTopic2 ] True
            , Link profile1 someAnswerTitle3 someUrl Answer [ someTopic3 ] False
            , Link profile1 someAnswerTitle4 someUrl Answer [ someTopic4 ] True
            , Link profile1 someAnswerTitle5 someUrl Answer [ someTopic5 ] False
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


provider : Id -> Maybe Provider
provider id =
    if id == profileId1 then
        Just provider1
    else if id == profileId2 then
        Just provider2
    else if id == profileId3 then
        Just provider3
    else if id == profileId4 then
        Just provider4
    else if id == profileId5 then
        Just provider5
    else
        Nothing


providers : List Provider
providers =
    [ provider1
    , provider2
    , provider3
    , provider4
    , provider5
    ]


topics : List Topic
topics =
    [ someTopic1, someTopic2, someTopic3, someTopic4, someTopic5 ]


topicLinks : Topic -> ContentType -> Id -> List Link
topicLinks topic contentType id =
    id
        |> linksToContent contentType
        |> List.filter (\link -> link.topics |> List.any (\t -> t.name == topic.name))


sources : Id -> List Source
sources profileId =
    [ { platform = "WordPress", username = "bizmonger", linksFound = 0 }
    , { platform = "YouTube", username = "bizmonger", linksFound = 0 }
    , { platform = "StackOverflow", username = "scott-nimrod", linksFound = 0 }
    ]


addSource : Id -> Source -> Result String (List Source)
addSource profileId connection =
    Ok <| connection :: (profileId |> sources)


removeSource : Id -> Source -> Result String (List Source)
removeSource profileId connection =
    Ok (profileId |> sources |> List.filter (\c -> profileId |> sources |> List.member connection))


usernameToId : String -> Id
usernameToId email =
    case email of
        "test" ->
            profileId1

        "profile_1" ->
            profileId1

        "profile_2" ->
            profileId2

        "profile_3" ->
            profileId3

        "profile_4" ->
            profileId4

        "profile_5" ->
            profileId5

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


follow : Id -> Id -> Result String ()
follow clientId providerId =
    Err "follow not implemented"


unsubscribe : Id -> Id -> Result String ()
unsubscribe clientId providerId =
    Err "unsubscribe not implemented"
