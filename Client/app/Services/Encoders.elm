module Services.Encoders exposing (..)

import Domain.Core exposing (..)
import Services.Adapter exposing (..)
import Json.Encode as Encode


encodeRegistration : Form -> Encode.Value
encodeRegistration form =
    Encode.object
        [ ( "FirstName", Encode.string form.firstName )
        , ( "LastName", Encode.string form.lastName )
        , ( "Email", Encode.string form.email )
        , ( "Password", Encode.string form.password )
        ]


encodeLink : Link -> Encode.Value
encodeLink link =
    Encode.object
        [ ( "Id", Encode.int <| link.id )
        , ( "ProfileId", Encode.string (idText link.profileId) )
        , ( "Title", Encode.string <| titleText link.title )
        , ( "Url", Encode.string <| urlText link.url )
        , ( "Topics", Encode.list (link.topics |> List.map encodeTopic) )
        , ( "ContentType", Encode.string <| contentTypeToText link.contentType )
        , ( "IsFeatured", Encode.bool link.isFeatured )
        ]


encodeLinks : ProviderLinks -> Encode.Value
encodeLinks providerLinks =
    let
        (ProviderLinks linkFields) =
            providerLinks
    in
        Encode.object
            [ ( "Links", Encode.list (linkFields.links |> List.map encodeLink) ) ]


encodeProfile : Profile -> Encode.Value
encodeProfile profile =
    let
        jsonProfile =
            profile |> toJsonProfile
    in
        Encode.object
            [ ( "ProfileId", Encode.string jsonProfile.id )
            , ( "FirstName", Encode.string jsonProfile.firstName )
            , ( "LastName", Encode.string jsonProfile.lastName )
            , ( "Email", Encode.string jsonProfile.email )
            , ( "ImageUrl", Encode.string jsonProfile.imageUrl )
            , ( "Bio", Encode.string jsonProfile.bio )
            , ( "Sources", Encode.list (profile.sources |> List.map encodeSource) )
            ]


encodeSource : Source -> Encode.Value
encodeSource source =
    Encode.object
        [ ( "Id"
          , Encode.int <|
                case source.id |> idText |> String.toInt of
                    Ok id ->
                        id

                    Err _ ->
                        -1
          )
        , ( "ProfileId", Encode.string <| idText source.profileId )
        , ( "Platform", Encode.string source.platform )
        , ( "AccessId", Encode.string source.username )

        -- , ( "Links", encodeLinks source.links )
        ]


encodeId : Id -> Encode.Value
encodeId id =
    Encode.object
        [ ( "Id", Encode.string <| idText id ) ]


encodeTopic : Topic -> Encode.Value
encodeTopic topic =
    Encode.object
        [ ( "Name", Encode.string <| topic.name )
        , ( "IsFeatured", Encode.bool <| topic.isFeatured )
        ]


encodeProviderWithTopic : Id -> Topic -> Encode.Value
encodeProviderWithTopic id topic =
    Encode.object
        [ ( "Id", Encode.string <| idText id )
        , ( "Topic", Encode.string <| topicText topic )
        ]


encodeCredentials : Credentials -> Encode.Value
encodeCredentials credentials =
    Encode.object
        [ ( "Email", Encode.string credentials.email )
        , ( "Password", Encode.string credentials.password )
        ]
