module Settings exposing (..)

import Domain.Core exposing (..)
import Tests.TestAPI as TestAPI exposing (..)
import Services.Server as Services exposing (..)


configuration : Configuration
configuration =
    Integration


type Configuration
    = Integration
    | Isolation


type alias Dependencies =
    { tryLogin : Loginfunction
    , contributors : Contributorsfunction
    , contributor : Contributorfunction
    , links : ContentTypefunction
    , topicLinks : TopicLinksfunction
    , usernameToId : UserNameToIdfunction
    , connections : Connectionsfunction
    , platforms : List Platform
    }


runtime : Dependencies
runtime =
    case configuration of
        Integration ->
            Dependencies
                Services.tryLogin
                Services.contributors
                Services.contributor
                Services.links
                Services.topicLinks
                Services.usernameToId
                Services.connections
                Services.platforms

        Isolation ->
            Dependencies
                TestAPI.tryLogin
                TestAPI.contributors
                TestAPI.contributor
                TestAPI.links
                TestAPI.topicLinks
                TestAPI.usernameToId
                TestAPI.connections
                TestAPI.platforms
