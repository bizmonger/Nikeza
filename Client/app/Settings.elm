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
    { bootstrap : Bootstrapfunction msg
    , tryLogin : Loginfunction msg
    , tryRegister : Registerfunction msg
    , updateProfile : UpdateProfilefunction msg
    , provider : Providerfunction msg
    , providerTopic : ProviderTopicfunction msg
    , providers : Providersfunction msg
    , portfolio : Portfoliofunction msg
    , addLink : AddLinkfunction msg
    , removeLink : RemoveLinkfunction msg
    , topicLinks : TopicLinksfunction msg
    , sources : Sourcesfunction msg
    , addSource : AddSourcefunction msg
    , removeSource : RemoveSourcefunction msg
    , suggestedTopics : SuggestedTopicsfunction msg
    , subscriptions : Subscriptionsfunction msg
    , followers : Followersfunction msg
    , follow : Followfunction msg
    , unsubscribe : Unsubscribefunction msg
    , recentLinks : RecentLinksfunction msg
    }


runtime : Dependencies msg
runtime =
    case configuration of
        Integration ->
            Dependencies
                Services.bootstrap
                Services.tryLogin
                Services.tryRegister
                Services.updateProfile
                Services.provider
                Services.providerTopic
                Services.providers
                Services.portfolio
                Services.addLink
                Services.removeLink
                Services.topicLinks
                Services.sources
                Services.addSource
                Services.removeSource
                Services.suggestedTopics
                Services.subscriptions
                Services.followers
                Services.follow
                Services.unsubscribe
                Services.recentLinks

        Isolation ->
            Dependencies
                TestAPI.bootstrap
                TestAPI.tryLogin
                TestAPI.tryRegister
                TestAPI.updateProfile
                TestAPI.provider
                TestAPI.providerTopic
                TestAPI.providers
                TestAPI.portfolio
                TestAPI.addLink
                TestAPI.removeLink
                TestAPI.topicLinks
                TestAPI.sources
                TestAPI.addSource
                TestAPI.removeSource
                TestAPI.suggestedTopics
                TestAPI.subscriptions
                TestAPI.followers
                TestAPI.follow
                TestAPI.unsubscribe
                TestAPI.recentLinks
