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
    , latestLinks : LatestLinksfunction
    , recentContributors : Contributorsfunction
    , contributor : Contributorfunction
    , links : ContentTypefunction
    , topicLinks : TopicLinksfunction
    }


runtime : Dependencies
runtime =
    case configuration of
        Integration ->
            Dependencies
                Services.tryLogin
                Services.latestLinks
                Services.recentContributors
                Services.contributor
                Services.links
                Services.topicLinks

        Isolation ->
            Dependencies
                TestAPI.tryLogin
                TestAPI.latestLinks
                TestAPI.recentContributors
                TestAPI.contributor
                TestAPI.links
                TestAPI.topicLinks
