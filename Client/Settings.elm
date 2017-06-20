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
    , contributor : Contributorfunction
    , contributors : Contributorsfunction
    , links : Linksfunction
    , addLink : AddLinkfunction
    , topicLinks : TopicLinksfunction
    , usernameToId : UserNameToIdfunction
    , connections : Connectionsfunction
    , addConnection : AddConnectionfunction
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
                Services.contributor
                Services.contributors
                Services.links
                Services.addLink
                Services.topicLinks
                Services.usernameToId
                Services.connections
                Services.addConnection
                Services.platforms
                Services.topics
                Services.suggestedTopics

        Isolation ->
            Dependencies
                TestAPI.tryLogin
                TestAPI.contributor
                TestAPI.contributors
                TestAPI.links
                TestAPI.addLink
                TestAPI.topicLinks
                TestAPI.usernameToId
                TestAPI.connections
                TestAPI.addConnection
                TestAPI.platforms
                TestAPI.topics
                TestAPI.suggestedTopics
