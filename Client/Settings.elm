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
    , topicUrl : TopicUrlfunction
    , contributorUrl : ContributorUrlfunction
    , latestPosts : LatestPostsfunction
    , recentContributors : Contributorsfunction
    , getContributor : GetContributorfunction
    , posts : ContentTypefunction
    }


runtime : Dependencies
runtime =
    case configuration of
        Integration ->
            Dependencies
                Services.tryLogin
                Services.topicUrl
                Services.contributorUrl
                Services.latestPosts
                Services.recentContributors
                Services.getContributor
                Services.posts

        Isolation ->
            Dependencies
                TestAPI.tryLogin
                TestAPI.topicUrl
                TestAPI.contributorUrl
                TestAPI.latestPosts
                TestAPI.recentContributors
                TestAPI.getContributor
                TestAPI.posts
