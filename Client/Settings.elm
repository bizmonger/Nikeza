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
    , contentProvider : ContentProviderfunction
    , contentProviders : ContentProvidersfunction
    , links : Linksfunction
    , addLink : AddLinkfunction
    , removeLink : RemoveLinkfunction
    , topicLinks : TopicLinksfunction
    , usernameToId : UserNameToIdfunction
    , connections : Connectionsfunction
    , addConnection : AddConnectionfunction
    , removeConnection : RemoveConnectionfunction
    , platforms : List Platform
    , topics : List Topic
    , suggestedTopics : SuggestedTopicsfunction
    }


runtime : Dependencies
runtime =
    case configuration of
        Integration ->
            Dependencies
                Services.tryLogin
                Services.contentProvider
                Services.contentProviders
                Services.links
                Services.addLink
                Services.removeLink
                Services.topicLinks
                Services.usernameToId
                Services.connections
                Services.addConnection
                Services.removeConnection
                Services.platforms
                Services.topics
                Services.suggestedTopics

        Isolation ->
            Dependencies
                TestAPI.tryLogin
                TestAPI.contentProvider
                TestAPI.contentProviders
                TestAPI.links
                TestAPI.addLink
                TestAPI.removeLink
                TestAPI.topicLinks
                TestAPI.usernameToId
                TestAPI.connections
                TestAPI.addConnection
                TestAPI.removeConnection
                TestAPI.platforms
                TestAPI.topics
                TestAPI.suggestedTopics
