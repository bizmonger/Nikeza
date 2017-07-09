module Settings exposing (..)

import Domain.Core exposing (..)
import Tests.TestAPI as TestAPI exposing (..)
import Services.Server as Services exposing (..)


configuration : Configuration
configuration =
    Isolation


type Configuration
    = Integration
    | Isolation


type alias Dependencies =
    { tryLogin : Loginfunction
    , tryRegister : Registerfunction
    , contentProvider : ContentProviderfunction
    , contentProviders : ContentProvidersfunction
    , links : Linksfunction
    , addLink : AddLinkfunction
    , removeLink : RemoveLinkfunction
    , topicLinks : TopicLinksfunction
    , usernameToId : UserNameToIdfunction
    , sources : Sourcesfunction
    , addSource : AddSourcefunction
    , removeSource : RemoveSourcefunction
    , platforms : List Platform
    , topics : List Topic
    , suggestedTopics : SuggestedTopicsfunction
    , subscribers : Subscribersfunction
    , followers : Followersfunction
    }


runtime : Dependencies
runtime =
    case configuration of
        Integration ->
            Dependencies
                Services.tryLogin
                Services.tryRegister
                Services.contentProvider
                Services.contentProviders
                Services.links
                Services.addLink
                Services.removeLink
                Services.topicLinks
                Services.usernameToId
                Services.sources
                Services.addSource
                Services.removeSource
                Services.platforms
                Services.topics
                Services.suggestedTopics
                Services.subscribers
                Services.followers

        Isolation ->
            Dependencies
                TestAPI.tryLogin
                TestAPI.tryRegister
                TestAPI.contentProvider
                TestAPI.contentProviders
                TestAPI.links
                TestAPI.addLink
                TestAPI.removeLink
                TestAPI.topicLinks
                TestAPI.usernameToId
                TestAPI.sources
                TestAPI.addSource
                TestAPI.removeSource
                TestAPI.platforms
                TestAPI.topics
                TestAPI.suggestedTopics
                TestAPI.subscribers
                TestAPI.followers
