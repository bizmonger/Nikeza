module Services.Decoders exposing (..)

import Services.Adapter exposing (..)
import Json.Decode as Decode exposing (Decoder, field)


profileDecoder : Decoder JsonProfile
profileDecoder =
    Decode.map7 JsonProfile
        (field "id" Decode.string)
        (field "firstName" Decode.string)
        (field "lastName" Decode.string)
        (field "email" Decode.string)
        (field "imageUrl" Decode.string)
        (field "bio" Decode.string)
        (field "sources" <| Decode.list sourceDecoder)


sourceDecoder : Decoder JsonSource
sourceDecoder =
    Decode.map5 JsonSource
        (field "id" Decode.int)
        (field "profileId" Decode.string)
        (field "platform" Decode.string)
        (field "accessId" Decode.string)
        (field "links" (Decode.list linkDecoder))


providerLinksDecoder : Decoder JsonProviderLinks
providerLinksDecoder =
    Decode.map JsonLinkFields
        (field "links" <| Decode.list (Decode.lazy (\_ -> linkDecoder)))
        |> Decode.map JsonProviderLinks


topicDecoder : Decoder JsonTopic
topicDecoder =
    Decode.map2 JsonTopic
        (field "name" Decode.string)
        (field "isFeatured" Decode.bool)


portfolioDecoder : Decoder JsonPortfolio
portfolioDecoder =
    Decode.map4 JsonPortfolio
        (field "answers" <| Decode.list linkDecoder)
        (field "articles" <| Decode.list linkDecoder)
        (field "videos" <| Decode.list linkDecoder)
        (field "podcasts" <| Decode.list linkDecoder)


bootstrapDecoder : Decoder JsonBootstrap
bootstrapDecoder =
    Decode.map2 JsonBootstrap
        (field "providers" <| Decode.list providerDecoder)
        (field "platforms" <| Decode.list Decode.string)


thumbnailDecoder : Decoder JsonThumbnail
thumbnailDecoder =
    Decode.map2 JsonThumbnail
        (field "imageUrl" Decode.string)
        (field "platform" Decode.string)


linkDecoder : Decoder JsonLink
linkDecoder =
    Decode.map8 JsonLink
        (field "id" Decode.int)
        (field "profileId" Decode.string)
        (field "title" Decode.string)
        (field "url" Decode.string)
        (field "contentType" Decode.string)
        (field "topics" <| Decode.list topicDecoder)
        (field "isFeatured" Decode.bool)
        (field "timestamp" Decode.string)


providerDecoder : Decoder JsonProvider
providerDecoder =
    Decode.map6 JsonProviderFields
        (field "profile" profileDecoder)
        (field "topics" <| Decode.list topicDecoder)
        (field "portfolio" <| portfolioDecoder)
        (field "recentLinks" <| Decode.list linkDecoder)
        (field "subscriptions" <| Decode.list Decode.string)
        (field "followers" <| Decode.list Decode.string)
        |> Decode.map JsonProvider


subscriptionActionResponseDecoder : Decoder JsonSubscriptionActionResponse
subscriptionActionResponseDecoder =
    Decode.map2 JsonSubscriptionActionResponse
        (field "user" providerDecoder)
        (field "provider" providerDecoder)
