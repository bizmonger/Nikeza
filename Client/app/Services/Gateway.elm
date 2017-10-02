module Services.Gateway exposing (..)

import Domain.Core exposing (..)
import Services.Adapter exposing (..)
import Http exposing (getString)
import Task exposing (succeed, perform)
import Json.Decode as Decode exposing (Decoder, field)
import Json.Encode as Encode


-- DECODERS/ENCODERS


profileDecoder : Decoder JsonProfile
profileDecoder =
    Decode.map7 JsonProfile
        (field "Id" Decode.string)
        (field "FirstName" Decode.string)
        (field "LastName" Decode.string)
        (field "Email" Decode.string)
        (field "ImageUrl" Decode.string)
        (field "Bio" Decode.string)
        (field "Sources" <| Decode.list sourceDecoder)


sourceDecoder : Decoder JsonSource
sourceDecoder =
    Decode.map3 JsonSource
        (field "Platform" Decode.string)
        (field "Usename" Decode.string)
        (field "LinksFound" Decode.int)


topicDecoder : Decoder JsonTopic
topicDecoder =
    Decode.map2 JsonTopic
        (field "Name" Decode.string)
        (field "IsFeatured" Decode.bool)


linkDecoder : Decoder JsonLink
linkDecoder =
    Decode.map6 JsonLink
        (field "Profile" profileDecoder)
        (field "Title" Decode.string)
        (field "Url" Decode.string)
        (field "ContentType" Decode.string)
        (field "Topics" <| Decode.list topicDecoder)
        (field "IsFeatured" Decode.bool)


listOfLinksDecoder : Decoder (List JsonLink)
listOfLinksDecoder =
    Decode.list linkDecoder


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


encodeRegistration : Form -> Encode.Value
encodeRegistration form =
    Encode.object
        [ ( "FirstName", Encode.string form.firstName )
        , ( "LastName", Encode.string form.firstName )
        , ( "Email", Encode.string form.email )
        , ( "Password", Encode.string form.password )
        ]


encodeId : Id -> Encode.Value
encodeId id =
    Encode.object
        [ ( "Id", Encode.string <| getId id ) ]


encodeTopic : Topic -> Encode.Value
encodeTopic topic =
    Encode.object
        [ ( "Name", Encode.string <| topic.name )
        , ( "IsFeatured", Encode.bool <| topic.isFeatured )
        ]


encodeProviderWithTopic : Id -> Topic -> Encode.Value
encodeProviderWithTopic id topic =
    Encode.object
        [ ( "Id", Encode.string <| getId id )
        , ( "Topic", Encode.string <| getTopic topic )
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
            baseUrl ++ (getId profileId) ++ "links"

        body =
            (encodeId profileId) |> Http.jsonBody

        request =
            Http.post url body linksDecoder
    in
        Http.send msg request


topicLinks : Id -> Topic -> ContentType -> (Result Http.Error (List JsonLink) -> msg) -> Cmd msg
topicLinks providerId topic contentType msg =
    let
        url =
            baseUrl ++ (getId providerId) ++ "/" ++ "topiclinks"

        body =
            encodeId providerId |> Http.jsonBody

        request =
            Http.post url body listOfLinksDecoder
    in
        Http.send msg request


addLink : Id -> Link -> Result String Portfolio
addLink profileId link =
    Err "Not implemented"


removeLink : Id -> Link -> Result String Portfolio
removeLink profileId link =
    Err "Not implemented"


usernameToId : String -> Id
usernameToId username =
    Id "undefined"


sources : Id -> List Source
sources profileId =
    []


addSource : Id -> Source -> Result String (List Source)
addSource profileId connection =
    Err "Not implemented"


removeSource : Id -> Source -> Result String (List Source)
removeSource profileId connection =
    Err "Not implemented"


platforms : List Platform
platforms =
    []


suggestedTopics : String -> List Topic
suggestedTopics search =
    []


subscriptions : Id -> (Result Http.Error Members -> msg) -> Cmd msg
subscriptions profileId msg =
    Members []
        |> Result.Ok
        |> msg
        |> Task.succeed
        |> Task.perform identity


followers : Id -> (Result Http.Error Members -> msg) -> Cmd msg
followers profileId msg =
    Members []
        |> Result.Ok
        |> msg
        |> Task.succeed
        |> Task.perform identity


follow : Id -> Id -> Result String ()
follow clientId providerId =
    Err "follow not implemented"


unsubscribe : Id -> Id -> Result String ()
unsubscribe clientId providerId =
    Err "unsubscribe not implemented"
