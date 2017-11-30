module Settings exposing (..)

import Tests.TestAPI as TestAPI exposing (..)
import Services.Gateway as Services exposing (..)
import Services.Adapter as Adapter exposing (..)


configuration : Configuration
configuration =
    Connected


type Configuration
    = Connected
    | Disconnected


type alias Dependencies msg =
    { bootstrap : Bootstrapfunction msg
    , tryLogin : Loginfunction msg
    , tryRegister : Registerfunction msg
    , updateProfile : UpdateProfilefunction msg
    , updateProfileAndTopics : UpdateProfileAndTopicsfunction msg
    , thumbnail : ThumbnailFunction msg
    , updateThumbnail : SaveThumbnailfunction msg
    , provider : Providerfunction msg
    , featuredTopics : FeaturedTopicsfunction msg
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

    -- , recentLinks : RecentLinksfunction msg
    , recentLinkProviders : RecentLinkProvidersfunction msg
    , featureLink : FeatureLinkfunction msg
    }


runtime : Dependencies msg
runtime =
    case configuration of
        Connected ->
            Dependencies
                Services.bootstrap
                Services.tryLogin
                Services.tryRegister
                Services.updateProfile
                Services.updateProfileAndTopics
                Services.thumbnail
                Services.updateThumbnail
                Services.provider
                Services.featuredTopics
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
                -- Services.recentLinks
                Services.recentLinkProviders
                Services.featureLink

        Disconnected ->
            Dependencies
                TestAPI.bootstrap
                TestAPI.tryLogin
                TestAPI.tryRegister
                TestAPI.updateProfile
                TestAPI.updateProfileAndTopics
                TestAPI.thumbnail
                TestAPI.updateThumbnail
                TestAPI.provider
                TestAPI.featuredTopics
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
                -- TestAPI.recentLinks
                TestAPI.recentLinkProviders
                TestAPI.featureLink
