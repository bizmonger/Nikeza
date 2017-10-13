module Tests.TestAPI exposing (..)

import Domain.Core as Domain exposing (..)
import String exposing (..)
import Services.Adapter exposing (..)
import Http exposing (Error)


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


provider1Portfolio : Portfolio
provider1Portfolio =
    Portfolio (answers profileId1) (articles profileId1) (videos profileId1) (podcasts profileId1)


provider2Portfolio : Portfolio
provider2Portfolio =
    Portfolio (answers profileId2) (articles profileId2) (videos profileId2) (podcasts profileId2)


provider3Portfolio : Portfolio
provider3Portfolio =
    Portfolio (answers profileId3) (articles profileId3) (videos profileId3) (podcasts profileId3)


provider4Portfolio : Portfolio
provider4Portfolio =
    Portfolio (answers profileId4) (articles profileId4) (videos profileId4) (podcasts profileId4)


provider5Portfolio : Portfolio
provider5Portfolio =
    Portfolio (answers profileId5) (articles profileId5) (videos profileId5) (podcasts profileId5)


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
    Profile profileId1 (Name "Scott") (Name "Nimrod") someEmail profile1ImageUrl someDescrtiption (profileId1 |> sourcesBase)


profile2 : Profile
profile2 =
    Profile profileId2 (Name "Pablo") (Name "Rivera") someEmail profile2ImageUrl someDescrtiption (profileId2 |> sourcesBase)


profile3 : Profile
profile3 =
    Profile profileId3 (Name "Adam") (Name "Wright") someEmail profile3ImageUrl someDescrtiption (profileId3 |> sourcesBase)


profile4 : Profile
profile4 =
    Profile profileId4 (Name "Mitchell") (Name "Tilbrook") someEmail profile4ImageUrl someDescrtiption (profileId4 |> sourcesBase)


profile5 : Profile
profile5 =
    Profile profileId5 (Name "Ody") (Name "Mbegbu") someEmail profile5ImageUrl someDescrtiption (profileId5 |> sourcesBase)


jsonPortfolio : Id -> JsonPortfolio
jsonPortfolio id =
    JsonPortfolio
        (answers id |> toJsonLinks)
        (articles id |> toJsonLinks)
        (videos id |> toJsonLinks)
        (podcasts id |> toJsonLinks)


subscriptions : Id -> (Result Http.Error Members -> msg) -> Cmd msg
subscriptions profileId msg =
    if profileId == profileId1 then
        -- Weird error when we have provider3
        Members [ provider2 ] |> httpSuccess msg
    else
        Members [] |> httpSuccess msg


followers : Id -> (Result Http.Error Members -> msg) -> Cmd msg
followers profileId msg =
    if profileId == profileId1 then
        Members [ provider4, provider5 ] |> httpSuccess msg
    else if profileId == profileId2 then
        Members [ provider1B, provider5 ] |> httpSuccess msg
    else if profileId == profileId3 then
        Members [ provider4, provider5 ] |> httpSuccess msg
    else
        Members [] |> httpSuccess msg


recentLinks1 : List Link
recentLinks1 =
    [ Link profile1 someVideoTitle6 someUrl Video [ someTopic1 ] True
    ]


recentLinks2 : List Link
recentLinks2 =
    [ Link profile2 somePodcastTitle6 someUrl Video [ someTopic1 ] True
    , Link profile2 someAnswerTitle6 someUrl Video [ someTopic1 ] True
    ]


recentLinks3 : List Link
recentLinks3 =
    [ Link profile3 someArticleTitle6 someUrl Video [ someTopic1 ] True
    ]


jsonProfile1 : JsonProfile
jsonProfile1 =
    JsonProfile
        (profileId1 |> idText)
        (profile1.firstName |> nameText)
        (profile1.lastName |> nameText)
        (profile1.email |> emailText)
        (profile1.imageUrl |> urlText)
        profile1.bio
        profile1.sources


jsonProfile2 : JsonProfile
jsonProfile2 =
    JsonProfile
        (profileId2 |> idText)
        (profile2.firstName |> nameText)
        (profile2.lastName |> nameText)
        (profile2.email |> emailText)
        (profile2.imageUrl |> urlText)
        profile2.bio
        profile2.sources


jsonProfile3 : JsonProfile
jsonProfile3 =
    JsonProfile
        (profileId3 |> idText)
        (profile3.firstName |> nameText)
        (profile3.lastName |> nameText)
        (profile3.email |> emailText)
        (profile3.imageUrl |> urlText)
        profile3.bio
        profile3.sources


jsonProfile4 : JsonProfile
jsonProfile4 =
    JsonProfile
        (profileId4 |> idText)
        (profile4.firstName |> nameText)
        (profile4.lastName |> nameText)
        (profile4.email |> emailText)
        (profile4.imageUrl |> urlText)
        profile4.bio
        profile4.sources


jsonProfile5 : JsonProfile
jsonProfile5 =
    JsonProfile
        (profileId5 |> idText)
        (profile5.firstName |> nameText)
        (profile5.lastName |> nameText)
        (profile5.email |> emailText)
        (profile5.imageUrl |> urlText)
        profile5.bio
        profile5.sources


jsonLink1 : JsonLink
jsonLink1 =
    JsonLink jsonProfile1 (titleText someArticleTitle1) (urlText someUrl) "video" [] False


jsonProvider1 : JsonProvider
jsonProvider1 =
    JsonProvider
        { profile = jsonProfile1
        , topics = topics
        , portfolio = profileId1 |> jsonPortfolio
        , recentLinks = (recentLinks1 |> toJsonLinks)
        , subscriptions = [ jsonProvider2, jsonProvider3 ]
        , followers = [ jsonProvider4, jsonProvider5 ]
        }


jsonProvider2 : JsonProvider
jsonProvider2 =
    JsonProvider
        { profile = jsonProfile2
        , topics = topics
        , portfolio = profileId2 |> jsonPortfolio
        , recentLinks = (recentLinks2 |> toJsonLinks)
        , subscriptions = []
        , followers = []
        }


jsonProvider3 : JsonProvider
jsonProvider3 =
    JsonProvider
        { profile = jsonProfile3
        , topics = topics
        , portfolio = profileId3 |> jsonPortfolio
        , recentLinks = (recentLinks3 |> toJsonLinks)
        , subscriptions = []
        , followers = []
        }


jsonProvider4 : JsonProvider
jsonProvider4 =
    JsonProvider
        { profile = jsonProfile4
        , topics = topics
        , portfolio = profileId4 |> jsonPortfolio
        , recentLinks = (recentLinks1 |> toJsonLinks)
        , subscriptions = []
        , followers = []
        }


jsonProvider5 : JsonProvider
jsonProvider5 =
    JsonProvider
        { profile = jsonProfile5
        , topics = topics
        , portfolio = profileId5 |> jsonPortfolio
        , recentLinks = (recentLinks1 |> toJsonLinks)
        , subscriptions = []
        , followers = []
        }


provider1 : Provider
provider1 =
    Provider profile1 topics provider1Portfolio provider1Portfolio recentLinks1 (Members []) (Members [])


provider1B : Provider
provider1B =
    Provider profile1 topics provider1Portfolio provider1Portfolio recentLinks1 (Members []) (Members [])


provider2 : Provider
provider2 =
    Provider profile2 topics provider2Portfolio provider2Portfolio recentLinks2 (Members []) (Members [])


provider3 : Provider
provider3 =
    Provider profile3 topics provider3Portfolio provider3Portfolio recentLinks3 (Members []) (Members [])


provider4 : Provider
provider4 =
    Provider profile4 topics provider4Portfolio provider4Portfolio [] (Members []) (Members [])


provider5 : Provider
provider5 =
    Provider profile5 topics provider5Portfolio provider5Portfolio [] (Members []) (Members [])



-- FUNCTIONS


tryLogin : Credentials -> (Result Http.Error JsonProvider -> msg) -> Cmd msg
tryLogin credentials msg =
    let
        successful =
            String.toLower credentials.email == "test" && String.toLower credentials.password == "test"
    in
        if successful then
            jsonProvider1 |> httpSuccess msg
        else
            Cmd.none


tryRegister : Form -> (Result Http.Error JsonProfile -> msg) -> Cmd msg
tryRegister form msg =
    if form.password == form.confirm then
        JsonProfile (idText profileId1) form.firstName form.lastName form.email "" "" []
            |> httpSuccess msg
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


links : Id -> (Result Http.Error JsonPortfolio -> msg) -> Cmd msg
links profileId msg =
    { answers = (profileId |> linksToContent Answer) |> toJsonLinks
    , articles = (profileId |> linksToContent Article) |> toJsonLinks
    , videos = (profileId |> linksToContent Video) |> toJsonLinks
    , podcasts = (profileId |> linksToContent Podcast) |> toJsonLinks
    }
        |> httpSuccess msg


addLink : Id -> Link -> (Result Http.Error JsonPortfolio -> msg) -> Cmd msg
addLink profileId link msg =
    JsonPortfolio [] [] [] []
        |> httpSuccess msg


removeLink : Id -> Link -> (Result Http.Error JsonPortfolio -> msg) -> Cmd msg
removeLink profileId link msg =
    JsonPortfolio [] [] [] []
        |> httpSuccess msg


linksToContent : ContentType -> Id -> List Link
linksToContent contentType profileId =
    let
        profileHolder =
            if profileId == profileId1 then
                profile1
            else if profileId == profileId2 then
                profile2
            else if profileId == profileId3 then
                profile3
            else if profileId == profileId4 then
                profile4
            else if profileId == profileId5 then
                profile5
            else
                profile1
    in
        -- NOTE !!! We're hardcoding a profile here due to some unresolved bug
        case contentType of
            Article ->
                [ Link profileHolder someArticleTitle1 someUrl Article [ someTopic1 ] False
                , Link profileHolder someArticleTitle2 someUrl Article [ someTopic2 ] True
                , Link profileHolder someArticleTitle3 someUrl Article [ someTopic3 ] False
                , Link profileHolder someArticleTitle4 someUrl Article [ someTopic4 ] True
                , Link profileHolder someArticleTitle5 someUrl Article [ someTopic5 ] False
                ]

            Video ->
                [ Link profileHolder someVideoTitle1 someUrl Video [ someTopic1 ] False
                , Link profileHolder someVideoTitle2 someUrl Video [ someTopic2 ] True
                , Link profileHolder someVideoTitle3 someUrl Video [ someTopic3 ] False
                , Link profileHolder someVideoTitle4 someUrl Video [ someTopic4 ] True
                , Link profileHolder someVideoTitle5 someUrl Video [ someTopic5 ] False
                ]

            Podcast ->
                [ Link profileHolder somePodcastTitle1 someUrl Podcast [ someTopic1 ] False
                , Link profileHolder somePodcastTitle2 someUrl Podcast [ someTopic2 ] True
                , Link profileHolder somePodcastTitle3 someUrl Podcast [ someTopic3 ] False
                , Link profileHolder somePodcastTitle4 someUrl Podcast [ someTopic4 ] True
                , Link profileHolder somePodcastTitle5 someUrl Podcast [ someTopic5 ] False
                ]

            Answer ->
                [ Link profileHolder someAnswerTitle1 someUrl Answer [ someTopic1 ] False
                , Link profileHolder someAnswerTitle2 someUrl Answer [ someTopic2 ] True
                , Link profileHolder someAnswerTitle3 someUrl Answer [ someTopic3 ] False
                , Link profileHolder someAnswerTitle4 someUrl Answer [ someTopic4 ] True
                , Link profileHolder someAnswerTitle5 someUrl Answer [ someTopic5 ] False
                ]

            All ->
                []

            Unknown ->
                []


suggestedTopics : String -> List Topic
suggestedTopics search =
    if not <| isEmpty search then
        topics |> List.filter (\t -> (topicText t) |> toLower |> contains (search |> toLower))
    else
        []


provider : Id -> (Result Http.Error JsonProvider -> msg) -> Cmd msg
provider id msg =
    if id == profileId1 then
        jsonProvider1 |> httpSuccess msg
    else if id == profileId2 then
        jsonProvider2 |> httpSuccess msg
    else if id == profileId3 then
        jsonProvider3 |> httpSuccess msg
    else if id == profileId4 then
        jsonProvider4 |> httpSuccess msg
    else if id == profileId5 then
        jsonProvider5 |> httpSuccess msg
    else
        Cmd.none


providerTopic : Id -> Topic -> (Result Http.Error JsonProvider -> msg) -> Cmd msg
providerTopic id topic msg =
    if id == profileId1 then
        jsonProvider1 |> httpSuccess msg
    else if id == profileId2 then
        jsonProvider2 |> httpSuccess msg
    else if id == profileId3 then
        jsonProvider3 |> httpSuccess msg
    else if id == profileId4 then
        jsonProvider4 |> httpSuccess msg
    else if id == profileId5 then
        jsonProvider5 |> httpSuccess msg
    else
        Cmd.none


providersBase : List JsonProvider
providersBase =
    [ jsonProvider1
    , jsonProvider2
    , jsonProvider3
    , jsonProvider4
    , jsonProvider5
    ]


providers : (Result Http.Error (List JsonProvider) -> msg) -> Cmd msg
providers msg =
    providersBase |> httpSuccess msg


topics : List Topic
topics =
    [ someTopic1, someTopic2, someTopic3, someTopic4, someTopic5 ]


topicLinks : Id -> Topic -> ContentType -> (Result Http.Error (List JsonLink) -> msg) -> Cmd msg
topicLinks profileId topic contentType msg =
    profileId
        |> linksToContent contentType
        |> List.filter (\link -> link.topics |> List.any (\t -> t.name == topic.name))
        |> toJsonLinks
        |> httpSuccess msg


sourcesBase : Id -> List Source
sourcesBase profileId =
    [ { platform = "WordPress", username = "bizmonger", linksFound = 0 }
    , { platform = "YouTube", username = "bizmonger", linksFound = 0 }
    , { platform = "StackOverflow", username = "scott-nimrod", linksFound = 0 }
    ]


sources : Id -> (Result Http.Error (List Source) -> msg) -> Cmd msg
sources profileId msg =
    profileId
        |> sourcesBase
        |> httpSuccess msg


addSourceBase : Id -> Source -> List Source
addSourceBase profileId source =
    source :: (profileId |> sourcesBase)


addSource : Id -> Source -> (Result Http.Error (List Source) -> msg) -> Cmd msg
addSource profileId source msg =
    source
        |> addSourceBase profileId
        |> httpSuccess msg


removeSourceBase : Id -> Source -> List JsonSource
removeSourceBase profileId source =
    profileId |> sourcesBase |> List.filter (\c -> profileId |> sourcesBase |> List.member source)


removeSource : Id -> Source -> (Result Http.Error (List JsonSource) -> msg) -> Cmd msg
removeSource profileId source msg =
    source
        |> removeSourceBase profileId
        |> httpSuccess msg


platformsBase : List Platform
platformsBase =
    [ Platform "WordPress"
    , Platform "YouTube"
    , Platform "Vimeo"
    , Platform "Medium"
    , Platform "StackOverflow"
    ]


platforms : (Result Http.Error (List Platform) -> msg) -> Cmd msg
platforms msg =
    platformsBase |> httpSuccess msg


providersAndPlatformsBase : ( List Provider, List Platform )
providersAndPlatformsBase =
    ( providersBase |> List.map toProvider, platformsBase )


providersAndPlatforms : (Result Http.Error ( List Provider, List Platform ) -> msg) -> Cmd msg
providersAndPlatforms msg =
    ( providersBase |> List.map toProvider, platformsBase )
        |> httpSuccess msg


follow : Id -> Id -> (Result Http.Error Members -> msg) -> Cmd msg
follow clientId providerId msg =
    Members [] |> httpSuccess msg


unsubscribe : Id -> Id -> (Result Http.Error Members -> msg) -> Cmd msg
unsubscribe clientId providerId msg =
    Members [] |> httpSuccess msg
