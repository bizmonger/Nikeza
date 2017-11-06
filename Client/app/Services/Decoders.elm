module Services.Decoders exposing (..)

import Services.Adapter exposing (..)
import Json.Decode as Decode exposing (Decoder, field)


profileDecoder : Decoder JsonProfile
profileDecoder =
    Decode.map7 JsonProfile
        (field "ProfileId" Decode.string)
        (field "FirstName" Decode.string)
        (field "LastName" Decode.string)
        (field "Email" Decode.string)
        (field "ImageUrl" Decode.string)
        (field "Bio" Decode.string)
        (field "Sources" <| Decode.list sourceDecoder)


sourceDecoder : Decoder JsonSource
sourceDecoder =
    Decode.map5 JsonSource
        (field "Id" Decode.int)
        (field "ProfileId" Decode.string)
        (field "Platform" Decode.string)
        (field "AccessId" Decode.string)
        (field "Links" (Decode.list linkDecoder))


providerLinksDecoder : Decoder JsonProviderLinks
providerLinksDecoder =
    Decode.map JsonLinkFields
        (field "Links" <| Decode.list (Decode.lazy (\_ -> linkDecoder)))
        |> Decode.map JsonProviderLinks


topicDecoder : Decoder JsonTopic
topicDecoder =
    Decode.map2 JsonTopic
        (field "Name" Decode.string)
        (field "IsFeatured" Decode.bool)


portfolioDecoder : Decoder JsonPortfolio
portfolioDecoder =
    Decode.map4 JsonPortfolio
        (field "Answers" <| Decode.list linkDecoder)
        (field "Articles" <| Decode.list linkDecoder)
        (field "Videos" <| Decode.list linkDecoder)
        (field "Podcasts" <| Decode.list linkDecoder)


bootstrapDecoder : Decoder JsonBootstrap
bootstrapDecoder =
    Decode.map2 JsonBootstrap
        (field "Providers" <| Decode.list providerDecoder)
        (field "Platforms" <| Decode.list Decode.string)


thumbnailDecoder : Decoder JsonThumbnail
thumbnailDecoder =
    Decode.map2 JsonThumbnail
        (field "ImageUrl" Decode.string)
        (field "Platform" Decode.string)


linkDecoder : Decoder JsonLink
linkDecoder =
    Decode.map7 JsonLink
        (field "Id" Decode.int)
        (field "ProfileId" Decode.string)
        (field "Title" Decode.string)
        (field "Url" Decode.string)
        (field "ContentType" Decode.string)
        (field "Topics" <| Decode.list topicDecoder)
        (field "IsFeatured" Decode.bool)


providerDecoder : Decoder JsonProvider
providerDecoder =
    Decode.map6 JsonProviderFields
        (field "Profile" profileDecoder)
        (field "Topics" <| Decode.list topicDecoder)
        (field "Portfolio" <| portfolioDecoder)
        (field "RecentLinks" <| Decode.list linkDecoder)
        (field "Subscriptions" <| Decode.list (Decode.lazy (\_ -> providerDecoder)))
        (field "Followers" <| Decode.list (Decode.lazy (\_ -> providerDecoder)))
        |> Decode.map JsonProvider
