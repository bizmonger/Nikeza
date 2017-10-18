module Services.Gateway exposing (..)

import Domain.Core exposing (..)
import Services.Adapter exposing (..)
import Http exposing (getString, Request, expectStringResponse, header)
import Json.Decode as Decode exposing (Decoder, field)
import Json.Encode as Encode


-- DECODERS


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
        (field "Username" Decode.string)
        (field "LinksFound" Decode.int)


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


linkDecoder : Decoder JsonLink
linkDecoder =
    Decode.map6 JsonLink
        (field "Profile" profileDecoder)
        (field "Title" Decode.string)
        (field "Url" Decode.string)
        (field "ContentType" Decode.string)
        (field "Topics" <| Decode.list topicDecoder)
        (field "IsFeatured" Decode.bool)


linksDecoder : Decoder JsonPortfolio
linksDecoder =
    Decode.map4 JsonPortfolio
        (field "Articles" <| Decode.list linkDecoder)
        (field "Videos" <| Decode.list linkDecoder)
        (field "Podcasts" <| Decode.list linkDecoder)
        (field "Answers" <| Decode.list linkDecoder)


providerDecoder : Decoder JsonProvider
providerDecoder =
    Decode.map6 JsonProviderFields
        (field "Profile" profileDecoder)
        (field "Topics" <| Decode.list topicDecoder)
        (field "Links" <| linksDecoder)
        (field "RecentLinks" <| Decode.list linkDecoder)
        (field "Subscriptions" <| Decode.list (Decode.lazy (\_ -> providerDecoder)))
        (field "Followers" <| Decode.list (Decode.lazy (\_ -> providerDecoder)))
        |> Decode.map JsonProvider



-- ENCODERS


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
        [ ( "Profile", encodeProfile link.profile )
        , ( "Title", Encode.string <| titleText link.title )
        , ( "Url", Encode.string <| urlText link.url )
        , ( "ContentType", Encode.string <| contentTypeToText link.contentType )
        , ( "Topics", Encode.list (link.topics |> List.map (\t -> encodeTopic t)) )
        , ( "IsFeatured", Encode.bool link.isFeatured )
        ]


encodeProfile : Profile -> Encode.Value
encodeProfile profile =
    let
        jsonProfile =
            profile |> toJsonProfile
    in
        Encode.object
            [ ( "ProfileId", Encode.string <| jsonProfile.id )
            , ( "FirstName", Encode.string <| jsonProfile.firstName )
            , ( "LastName", Encode.string <| jsonProfile.lastName )
            , ( "Email", Encode.string <| jsonProfile.email )
            , ( "ImageUrl", Encode.string <| jsonProfile.imageUrl )
            , ( "Bio", Encode.string <| jsonProfile.bio )
            , ( "Sources", Encode.list (profile.sources |> List.map (\s -> encodeSource s)) )
            ]


encodeSource : Source -> Encode.Value
encodeSource source =
    Encode.object
        [ ( "ProfileId", Encode.string <| idText source.profileId )
        , ( "Platform", Encode.string <| source.platform )
        , ( "Username", Encode.string <| source.username )
        , ( "LinksFound", Encode.int <| source.linksFound )
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



-- Requests


baseUrl : String
baseUrl =
    "http://localhost:5000/"


tryLogin : Credentials -> (Result Http.Error JsonProvider -> msg) -> Cmd msg
tryLogin credentials msg =
    let
        url =
            baseUrl ++ "login"

        body =
            encodeCredentials credentials |> Http.jsonBody

        request =
            Http.post url body providerDecoder
    in
        Http.send msg request


tryRegister : Form -> (Result Http.Error JsonProfile -> msg) -> Cmd msg
tryRegister form msg =
    let
        url =
            baseUrl ++ "register"

        body =
            encodeRegistration form |> Http.jsonBody

        request =
            Http.post url body profileDecoder
    in
        Http.send msg request


providers : (Result Http.Error (List JsonProvider) -> msg) -> Cmd msg
providers msg =
    let
        url =
            baseUrl ++ "providers"

        request =
            Http.get url (Decode.list providerDecoder)
    in
        Http.send msg request


provider : Id -> (Result Http.Error JsonProvider -> msg) -> Cmd msg
provider id msg =
    let
        url =
            baseUrl ++ "provider"

        body =
            encodeId id |> Http.jsonBody

        request =
            Http.post url body providerDecoder
    in
        Http.send msg request


providerTopic : Id -> Topic -> (Result Http.Error JsonProvider -> msg) -> Cmd msg
providerTopic id topic msg =
    let
        url =
            baseUrl ++ "providertopic"

        body =
            (encodeProviderWithTopic id topic) |> Http.jsonBody

        request =
            Http.post url body providerDecoder
    in
        Http.send msg request


links : Id -> (Result Http.Error JsonPortfolio -> msg) -> Cmd msg
links profileId msg =
    let
        url =
            baseUrl ++ (idText profileId) ++ "links"

        body =
            (encodeId profileId) |> Http.jsonBody

        request =
            Http.post url body linksDecoder
    in
        Http.send msg request


topicLinks : Id -> Topic -> ContentType -> (Result Http.Error (List JsonLink) -> msg) -> Cmd msg
topicLinks profileId topic contentType msg =
    let
        url =
            baseUrl ++ (idText profileId) ++ "/topiclinks"

        body =
            encodeId profileId |> Http.jsonBody

        request =
            Http.post url body (Decode.list linkDecoder)
    in
        Http.send msg request


addLink : Id -> Link -> (Result Http.Error JsonPortfolio -> msg) -> Cmd msg
addLink profileId link msg =
    let
        url =
            baseUrl ++ (idText profileId) ++ "/addlink"

        body =
            encodeLink link |> Http.jsonBody

        request =
            Http.post url body portfolioDecoder
    in
        Http.send msg request


removeLink : Id -> Link -> (Result Http.Error JsonPortfolio -> msg) -> Cmd msg
removeLink profileId link msg =
    let
        url =
            baseUrl ++ (idText profileId) ++ "/removelink"

        body =
            encodeLink link |> Http.jsonBody

        request =
            Http.post url body portfolioDecoder
    in
        Http.send msg request


sources : Id -> (Result Http.Error (List Source) -> msg) -> Cmd msg
sources profileId msg =
    Cmd.none


addSource : Source -> (Result Http.Error JsonSource -> msg) -> Cmd msg
addSource source msg =
    let
        url =
            baseUrl ++ "addsource"

        body =
            encodeSource source |> Http.jsonBody

        request =
            Http.post url body sourceDecoder
    in
        Http.send msg request


removeSource : Id -> (Result Http.Error JsonSource -> msg) -> Cmd msg
removeSource sourceId msg =
    case (idText sourceId) |> String.toInt of
        Ok id ->
            let
                url =
                    baseUrl ++ "removesource/" ++ (id |> toString)

                body =
                    Encode.int id |> Http.jsonBody

                request =
                    Http.post url body sourceDecoder
            in
                Http.send msg request

        Err _ ->
            Cmd.none


bootstrap : (Result Http.Error JsonBootstrap -> msg) -> Cmd msg
bootstrap msg =
    let
        url =
            baseUrl ++ "bootstrap"

        request =
            Http.get url bootstrapDecoder
    in
        Http.send msg request


updateProfile : Profile -> (Result Http.Error JsonProfile -> msg) -> Cmd msg
updateProfile profile msg =
    let
        url =
            baseUrl ++ "updateprofile"

        body =
            encodeProfile profile |> Http.jsonBody

        request =
            Http.post url body profileDecoder
    in
        Http.send msg request


suggestedTopics : String -> List Topic
suggestedTopics search =
    []


subscriptions : Id -> (Result Http.Error Members -> msg) -> Cmd msg
subscriptions profileId msg =
    Members [] |> httpSuccess msg


followers : Id -> (Result Http.Error Members -> msg) -> Cmd msg
followers profileId msg =
    Members [] |> httpSuccess msg


follow : Id -> Id -> (Result Http.Error Members -> msg) -> Cmd msg
follow clientId providerId msg =
    Members [] |> httpSuccess msg


unsubscribe : Id -> Id -> (Result Http.Error Members -> msg) -> Cmd msg
unsubscribe clientId providerId msg =
    Members [] |> httpSuccess msg
