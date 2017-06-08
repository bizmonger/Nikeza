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
    , latestPosts : LatestPostsfunction
    , recentContributors : Contributorsfunction
    , contributor : Contributorfunction
    , posts : ContentTypefunction
    , topicPosts : TopicPostsfunction
    }


runtime : Dependencies
runtime =
    case configuration of
        Integration ->
            Dependencies
                Services.tryLogin
                Services.latestPosts
                Services.recentContributors
                Services.contributor
                Services.posts
                Services.topicPosts

        Isolation ->
            Dependencies
                TestAPI.tryLogin
                TestAPI.latestPosts
                TestAPI.recentContributors
                TestAPI.contributor
                TestAPI.posts
                TestAPI.topicPosts
