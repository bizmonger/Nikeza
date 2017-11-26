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
    Portfolio [] (answers profileId1) (articles profileId1) (videos profileId1) (podcasts profileId1)


provider2Portfolio : Portfolio
provider2Portfolio =
    Portfolio [] (answers profileId2) (articles profileId2) (videos profileId2) (podcasts profileId2)


provider3Portfolio : Portfolio
provider3Portfolio =
    Portfolio [] (answers profileId3) (articles profileId3) (videos profileId3) (podcasts profileId3)


provider4Portfolio : Portfolio
provider4Portfolio =
    Portfolio [] (answers profileId4) (articles profileId4) (videos profileId4) (podcasts profileId4)


provider5Portfolio : Portfolio
provider5Portfolio =
    Portfolio [] (answers profileId5) (articles profileId5) (videos profileId5) (podcasts profileId5)


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
    Profile profileId1 (Name "Scott") (Name "Nimrod") someEmail profile1ImageUrl someDescrtiption sourcesBase


profile2 : Profile
profile2 =
    Profile profileId2 (Name "Pablo") (Name "Rivera") someEmail profile2ImageUrl someDescrtiption sourcesBase


profile3 : Profile
profile3 =
    Profile profileId3 (Name "Adam") (Name "Wright") someEmail profile3ImageUrl someDescrtiption sourcesBase


profile4 : Profile
profile4 =
    Profile profileId4 (Name "Mitchell") (Name "Tilbrook") someEmail profile4ImageUrl someDescrtiption sourcesBase


profile5 : Profile
profile5 =
    Profile profileId5 (Name "Ody") (Name "Mbegbu") someEmail profile5ImageUrl someDescrtiption sourcesBase


jsonPortfolio : Id -> JsonPortfolio
jsonPortfolio id =
    JsonPortfolio
        (answers id |> toJsonLinks)
        (articles id |> toJsonLinks)
        (videos id |> toJsonLinks)
        (podcasts id |> toJsonLinks)


subscriptions : Id -> (Result Http.Error (List JsonProvider) -> msg) -> Cmd msg
subscriptions profileId msg =
    if profileId == profileId1 then
        -- Weird error when we have provider3
        [ jsonProvider2 ] |> httpSuccess msg
    else
        [] |> httpSuccess msg


followers : Id -> (Result Http.Error (List JsonProvider) -> msg) -> Cmd msg
followers profileId msg =
    if profileId == profileId1 then
        [ jsonProvider4, jsonProvider5 ] |> httpSuccess msg
    else if profileId == profileId2 then
        [ jsonProvider1, jsonProvider5 ] |> httpSuccess msg
    else if profileId == profileId3 then
        [ jsonProvider4, jsonProvider5 ] |> httpSuccess msg
    else
        [] |> httpSuccess msg


recentLinks : Id -> (Result Http.Error (List JsonLink) -> msg) -> Cmd msg
recentLinks profileId msg =
    [ recentLinks1 |> toJsonLinks
    , recentLinks2 |> toJsonLinks
    ]
        |> List.concat
        |> httpSuccess msg


recentLinks1 : List Link
recentLinks1 =
    [ Link 0 profile1.id someVideoTitle6 someUrl [ someTopic1 ] Video True
    ]


recentLinks2 : List Link
recentLinks2 =
    [ Link 1 profile2.id somePodcastTitle6 someUrl [ someTopic1 ] Video True
    , Link 2 profile2.id someAnswerTitle6 someUrl [ someTopic1 ] Video True
    ]


recentLinks3 : List Link
recentLinks3 =
    [ Link 3 profile3.id someArticleTitle6 someUrl [ someTopic1 ] Video True
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
        (profile1.sources |> List.map (\s -> s |> toJsonSource))


jsonProfile2 : JsonProfile
jsonProfile2 =
    JsonProfile
        (profileId2 |> idText)
        (profile2.firstName |> nameText)
        (profile2.lastName |> nameText)
        (profile2.email |> emailText)
        (profile2.imageUrl |> urlText)
        profile2.bio
        (profile2.sources |> List.map (\s -> s |> toJsonSource))


jsonProfile3 : JsonProfile
jsonProfile3 =
    JsonProfile
        (profileId3 |> idText)
        (profile3.firstName |> nameText)
        (profile3.lastName |> nameText)
        (profile3.email |> emailText)
        (profile3.imageUrl |> urlText)
        profile3.bio
        (profile3.sources |> List.map (\s -> s |> toJsonSource))


jsonProfile4 : JsonProfile
jsonProfile4 =
    JsonProfile
        (profileId4 |> idText)
        (profile4.firstName |> nameText)
        (profile4.lastName |> nameText)
        (profile4.email |> emailText)
        (profile4.imageUrl |> urlText)
        profile4.bio
        (profile4.sources |> List.map (\s -> s |> toJsonSource))


jsonProfile5 : JsonProfile
jsonProfile5 =
    JsonProfile
        (profileId5 |> idText)
        (profile5.firstName |> nameText)
        (profile5.lastName |> nameText)
        (profile5.email |> emailText)
        (profile5.imageUrl |> urlText)
        profile5.bio
        (profile5.sources |> List.map (\s -> s |> toJsonSource))


jsonLink1 : JsonLink
jsonLink1 =
    JsonLink 0 (jsonProfile1.id) (titleText someArticleTitle1) (urlText someUrl) "video" [] False


jsonProvider1 : JsonProvider
jsonProvider1 =
    JsonProvider
        { profile = jsonProfile1
        , topics = topics
        , portfolio = profileId1 |> jsonPortfolio
        , recentLinks = (recentLinks1 |> toJsonLinks)
        , subscriptions = [ idText profile2.id, idText profile3.id ]
        , followers = [ idText profile4.id, idText profile5.id ]
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
    Provider profile1 topics provider1Portfolio provider1Portfolio recentLinks1 [] []


provider1B : Provider
provider1B =
    Provider profile1 topics provider1Portfolio provider1Portfolio recentLinks1 [] []


provider2 : Provider
provider2 =
    Provider profile2 topics provider2Portfolio provider2Portfolio recentLinks2 [] []


provider3 : Provider
provider3 =
    Provider profile3 topics provider3Portfolio provider3Portfolio recentLinks3 [] []


provider4 : Provider
provider4 =
    Provider profile4 topics provider4Portfolio provider4Portfolio [] [] []


provider5 : Provider
provider5 =
    Provider profile5 topics provider5Portfolio provider5Portfolio [] [] []



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


portfolio : Id -> (Result Http.Error JsonPortfolio -> msg) -> Cmd msg
portfolio profileId msg =
    { answers = (profileId |> linksToContent Answer) |> toJsonLinks
    , articles = (profileId |> linksToContent Article) |> toJsonLinks
    , videos = (profileId |> linksToContent Video) |> toJsonLinks
    , podcasts = (profileId |> linksToContent Podcast) |> toJsonLinks
    }
        |> httpSuccess msg


addLink : Link -> (Result Http.Error JsonLink -> msg) -> Cmd msg
addLink link msg =
    link
        |> toJsonLink
        |> httpSuccess msg


removeLink : Link -> (Result Http.Error JsonLink -> msg) -> Cmd msg
removeLink link msg =
    link
        |> toJsonLink
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
                [ Link 0 profileHolder.id someArticleTitle1 someUrl [ someTopic1 ] Article False
                , Link 1 profileHolder.id someArticleTitle2 someUrl [ someTopic2 ] Article True
                , Link 2 profileHolder.id someArticleTitle3 someUrl [ someTopic3 ] Article False
                , Link 3 profileHolder.id someArticleTitle4 someUrl [ someTopic4 ] Article True
                , Link 4 profileHolder.id someArticleTitle5 someUrl [ someTopic5 ] Article False
                ]

            Video ->
                [ Link 0 profileHolder.id someVideoTitle1 someUrl [ someTopic1 ] Video False
                , Link 1 profileHolder.id someVideoTitle2 someUrl [ someTopic2 ] Video True
                , Link 2 profileHolder.id someVideoTitle3 someUrl [ someTopic3 ] Video False
                , Link 3 profileHolder.id someVideoTitle4 someUrl [ someTopic4 ] Video True
                , Link 4 profileHolder.id someVideoTitle5 someUrl [ someTopic5 ] Video False
                ]

            Podcast ->
                [ Link 0 profileHolder.id somePodcastTitle1 someUrl [ someTopic1 ] Podcast False
                , Link 1 profileHolder.id somePodcastTitle2 someUrl [ someTopic2 ] Podcast True
                , Link 2 profileHolder.id somePodcastTitle3 someUrl [ someTopic3 ] Podcast False
                , Link 3 profileHolder.id somePodcastTitle4 someUrl [ someTopic4 ] Podcast True
                , Link 4 profileHolder.id somePodcastTitle5 someUrl [ someTopic5 ] Podcast False
                ]

            Answer ->
                [ Link 0 profileHolder.id someAnswerTitle1 someUrl [ someTopic1 ] Answer False
                , Link 1 profileHolder.id someAnswerTitle2 someUrl [ someTopic2 ] Answer True
                , Link 2 profileHolder.id someAnswerTitle3 someUrl [ someTopic3 ] Answer False
                , Link 3 profileHolder.id someAnswerTitle4 someUrl [ someTopic4 ] Answer True
                , Link 4 profileHolder.id someAnswerTitle5 someUrl [ someTopic5 ] Answer False
                ]

            All ->
                []

            Featured ->
                [ Link 1 profileHolder.id someArticleTitle2 someUrl [ someTopic2 ] Article True
                , Link 3 profileHolder.id someArticleTitle4 someUrl [ someTopic4 ] Article True
                , Link 1 profileHolder.id someVideoTitle2 someUrl [ someTopic2 ] Video True
                , Link 3 profileHolder.id someVideoTitle4 someUrl [ someTopic4 ] Video True
                , Link 1 profileHolder.id someAnswerTitle2 someUrl [ someTopic2 ] Answer True
                , Link 3 profileHolder.id someAnswerTitle4 someUrl [ someTopic4 ] Answer True
                , Link 1 profileHolder.id somePodcastTitle2 someUrl [ someTopic2 ] Podcast True
                , Link 3 profileHolder.id somePodcastTitle4 someUrl [ someTopic4 ] Podcast True
                ]

            Unknown ->
                []


suggestedTopics : String -> (Result Http.Error (List String) -> msg) -> Cmd msg
suggestedTopics search msg =
    if (not <| isEmpty search) && String.length search > 1 then
        topics
            |> List.map .name
            |> List.filter (\t -> t |> toLower |> contains (search |> toLower))
            |> httpSuccess msg
    else
        Cmd.none


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


featuredTopics : Id -> List String -> (Result Http.Error JsonProvider -> msg) -> Cmd msg
featuredTopics id topics msg =
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


sourcesBase : List Source
sourcesBase =
    [ { id = Id "0", profileId = Id "0", platform = "WordPress", accessId = "bizmonger", links = [] }
    , { id = Id "1", profileId = Id "1", platform = "YouTube", accessId = "bizmonger", links = [] }
    , { id = Id "2", profileId = Id "2", platform = "StackOverflow", accessId = "scott-nimrod", links = [] }
    ]


updateProfile : Profile -> (Result Http.Error JsonProfile -> msg) -> Cmd msg
updateProfile profile msg =
    profile |> toJsonProfile |> httpSuccess msg


featureLink : FeatureLink -> (Result Http.Error Int -> msg) -> Cmd msg
featureLink request msg =
    request.linkId |> httpSuccess msg


thumbnail : Platform -> String -> (Result Http.Error JsonThumbnail -> msg) -> Cmd msg
thumbnail platform accessId msg =
    { platform = "youtube", imageUrl = urlText someImageUrl } |> httpSuccess msg


updateThumbnail : UpdateThumbnailRequest -> (Result Http.Error String -> msg) -> Cmd msg
updateThumbnail request msg =
    someImageUrl |> urlText |> httpSuccess msg


sources : Id -> (Result Http.Error (List JsonSource) -> msg) -> Cmd msg
sources profileId msg =
    sourcesBase |> List.map toJsonSource |> httpSuccess msg


addSource : Source -> (Result Http.Error JsonSource -> msg) -> Cmd msg
addSource source msg =
    source |> toJsonSource |> httpSuccess msg


removeSourceBase : Id -> JsonSource
removeSourceBase sourceId =
    case sourceId |> idText |> String.toInt of
        Ok id ->
            let
                result =
                    sourcesBase
                        |> List.filter (\s -> idText s.id == (id |> toString))
                        |> List.head
            in
                case result of
                    Just source ->
                        source |> toJsonSource

                    Nothing ->
                        initSource |> toJsonSource

        Err _ ->
            initSource |> toJsonSource


removeSource : Id -> (Result Http.Error String -> msg) -> Cmd msg
removeSource sourceId msg =
    sourceId |> toString |> httpSuccess msg


platformsBase : List String
platformsBase =
    [ "WordPress"
    , "YouTube"
    , "Vimeo"
    , "Medium"
    , "StackOverflow"
    , "Rss Feed"
    ]


bootstrapBase : JsonBootstrap
bootstrapBase =
    { providers = providersBase
    , platforms = platformsBase
    }


bootstrap : (Result Http.Error JsonBootstrap -> msg) -> Cmd msg
bootstrap msg =
    { providers = providersBase
    , platforms = platformsBase
    }
        |> httpSuccess msg


follow : SubscriptionRequest -> (Result Http.Error JsonProvider -> msg) -> Cmd msg
follow request msg =
    provider2 |> toJsonProvider |> httpSuccess msg


unsubscribe : SubscriptionRequest -> (Result Http.Error JsonProvider -> msg) -> Cmd msg
unsubscribe request msg =
    provider2 |> toJsonProvider |> httpSuccess msg
