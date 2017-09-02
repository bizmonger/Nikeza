module Settings exposing (..)

import Domain.Core exposing (..)
import Tests.TestAPI as TestAPI exposing (..)
import Services.Gateway as Services exposing (..)
import Services.Adapter as Adapter exposing (..)


configuration : Configuration
configuration =
    Isolation


type Configuration
    = Integration
    | Isolation


type alias Dependencies msg =
    { tryLogin : Loginfunction msg
    , tryRegister : Registerfunction msg
    , provider : Providerfunction msg
    , providers : Providersfunction
    , links : Linksfunction
    , addLink : AddLinkfunction
    , removeLink : RemoveLinkfunction
    , topicLinks : TopicLinksfunction
    , usernameToId : UserNameToIdfunction
    , sources : Sourcesfunction
    , addSource : AddSourcefunction
    , removeSource : RemoveSourcefunction
    , platforms : List Platform
    , suggestedTopics : SuggestedTopicsfunction
    , subscriptions : Subscriptionsfunction
    , followers : Followersfunction
    , follow : Followfunction
    , unsubscribe : Unsubscribefunction
    }


runtime : Dependencies msg
runtime =
    case configuration of
        Integration ->
            Dependencies
                Services.tryLogin
                Services.tryRegister
                Services.provider
                Services.providers
                Services.links
                Services.addLink
                Services.removeLink
                Services.topicLinks
                Services.usernameToId
                Services.sources
                Services.addSource
                Services.removeSource
                Services.platforms
                Services.suggestedTopics
                Services.subscriptions
                Services.followers
                Services.follow
                Services.unsubscribe

        Isolation ->
            Dependencies
                TestAPI.tryLogin
                TestAPI.tryRegister
                TestAPI.provider
                TestAPI.providers
                TestAPI.links
                TestAPI.addLink
                TestAPI.removeLink
                TestAPI.topicLinks
                TestAPI.usernameToId
                TestAPI.sources
                TestAPI.addSource
                TestAPI.removeSource
                TestAPI.platforms
                TestAPI.suggestedTopics
                TestAPI.subscriptions
                TestAPI.followers
                TestAPI.follow
                TestAPI.unsubscribe
