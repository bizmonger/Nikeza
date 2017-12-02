module Services.Encoders exposing (..)

import Domain.Core exposing (..)
import Services.Adapter exposing (..)
import Json.Encode as Encode


encodeRegistration : Form -> Encode.Value
encodeRegistration form =
    Encode.object
        [ ( "firstName", Encode.string form.firstName )
        , ( "lastName", Encode.string form.lastName )
        , ( "email", Encode.string form.email )
        , ( "password", Encode.string form.password )
        ]


encodeThumbnailUpdate : UpdateThumbnailRequest -> Encode.Value
encodeThumbnailUpdate request =
    Encode.object
        [ ( "profileId", Encode.string <| idText request.profileId )
        , ( "imageUrl", Encode.string <| urlText request.imageUrl )
        ]


encodeFeatureLink : FeatureLink -> Encode.Value
encodeFeatureLink request =
    Encode.object
        [ ( "linkId", Encode.int <| request.linkId )
        , ( "isFeatured", Encode.bool <| request.isFeatured )
        ]


encodeLink : Link -> Encode.Value
encodeLink link =
    Encode.object
        [ ( "id", Encode.int <| link.id )
        , ( "profileId", Encode.string (idText link.profileId) )
        , ( "title", Encode.string <| titleText link.title )
        , ( "url", Encode.string <| urlText link.url )
        , ( "topics", Encode.list (link.topics |> List.map encodeTopic) )
        , ( "contentType", Encode.string <| contentTypeToText link.contentType )
        , ( "isFeatured", Encode.bool link.isFeatured )
        ]


encodeLinks : ProviderLinks -> Encode.Value
encodeLinks providerLinks =
    let
        (ProviderLinks linkFields) =
            providerLinks
    in
        Encode.object
            [ ( "links", Encode.list (linkFields.links |> List.map encodeLink) ) ]


encodeProfile : Profile -> Encode.Value
encodeProfile profile =
    let
        jsonProfile =
            profile |> toJsonProfile
    in
        Encode.object
            [ ( "id", Encode.string jsonProfile.id )
            , ( "firstName", Encode.string jsonProfile.firstName )
            , ( "lastName", Encode.string jsonProfile.lastName )
            , ( "email", Encode.string jsonProfile.email )
            , ( "imageUrl", Encode.string jsonProfile.imageUrl )
            , ( "bio", Encode.string jsonProfile.bio )
            , ( "sources", Encode.list (profile.sources |> List.map encodeSource) )
            ]


encodeProfileAndTopics : ProfileAndTopics -> Encode.Value
encodeProfileAndTopics profileAndTopics =
    Encode.object
        [ ( "profile", encodeProfile (profileAndTopics.profile) )
        , ( "topics", Encode.list (profileAndTopics.topics |> List.map (\t -> encodeTopic t)) )
        ]


encodeSubscriptionRequest : SubscriptionRequest -> Encode.Value
encodeSubscriptionRequest request =
    Encode.object
        [ ( "subscriberId", Encode.string <| idText request.subscriberId )
        , ( "profileId", Encode.string <| idText request.providerId )
        ]


encodeSource : Source -> Encode.Value
encodeSource source =
    Encode.object
        [ ( "id"
          , Encode.int <|
                case source.id |> idText |> String.toInt of
                    Ok id ->
                        id

                    Err _ ->
                        -1
          )
        , ( "profileId", Encode.string <| idText source.profileId )
        , ( "platform", Encode.string source.platform )
        , ( "accessId", Encode.string source.accessId )
        ]


encodeId : Id -> Encode.Value
encodeId id =
    Encode.object
        [ ( "id", Encode.string <| idText id ) ]


encodeTopic : Topic -> Encode.Value
encodeTopic topic =
    Encode.object
        [ ( "name", Encode.string <| topic.name )
        , ( "isFeatured", Encode.bool <| topic.isFeatured )
        ]


encodeFeaturedTopics : Id -> List String -> Encode.Value
encodeFeaturedTopics id topics =
    Encode.object
        [ ( "id", Encode.string <| idText id )
        , ( "topics", Encode.list (topics |> List.map Encode.string) )
        ]


encodeCredentials : Credentials -> Encode.Value
encodeCredentials credentials =
    Encode.object
        [ ( "email", Encode.string credentials.email )
        , ( "password", Encode.string credentials.password )
        ]
