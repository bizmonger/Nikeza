module Services.Gateway exposing (..)

import Domain.Core exposing (..)
import Services.Adapter exposing (..)
import Http exposing (getString)
import Json.Decode as Decode exposing (Decoder, field)
import Json.Encode as Encode


-- DECODERS/ENCODERS


profileDecoder : Decoder JsonProfile
profileDecoder =
    Decode.map4 JsonProfile
        (field "Id" Decode.string)
        (field "FirstName" Decode.string)
        (field "LastName" Decode.string)
        (field "Email" Decode.string)


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


linksDecoder : Decoder JsonLinks
linksDecoder =
    Decode.map4 JsonLinks
        (field "Articles" <| Decode.list linkDecoder)
        (field "Videos" <| Decode.list linkDecoder)
        (field "Podcasts" <| Decode.list linkDecoder)
        (field "Answers" <| Decode.list linkDecoder)


providerDecoder : Decoder JsonProvider
providerDecoder =
    Decode.map6 JsonProvider
        (field "Profile" profileDecoder)
        (field "Topics" <| Decode.list topicDecoder)
        (field "Links" <| linksDecoder)
        (field "RecentLinks" <| Decode.list linkDecoder)
        (field "Subscriptions" <| Decode.list {})
        (field "Followers" <| Decode.list {})


encodeRegistration : Form -> Encode.Value
encodeRegistration form =
    Encode.object
        [ ( "FirstName", Encode.string form.firstName )
        , ( "LastName", Encode.string form.firstName )
        , ( "Email", Encode.string form.email )
        , ( "Password", Encode.string form.password )
        ]


encodeCredentials : Credentials -> Encode.Value
encodeCredentials credentials =
    Encode.object
        [ ( "Email", Encode.string credentials.email )
        , ( "Password", Encode.string credentials.password )
        ]


tryLogin : Credentials -> (Result Http.Error JsonProvider -> msg) -> Cmd msg
tryLogin credentials msg =
    let
        loginUrl =
            "http://localhost:5000/login"

        body =
            encodeCredentials credentials |> Http.jsonBody

        request =
            Http.post loginUrl body providerDecoder
    in
        Http.send msg request


tryRegister : Form -> (Result Http.Error JsonProfile -> msg) -> Cmd msg
tryRegister form msg =
    let
        registerUrl =
            "http://localhost:5000/register"

        body =
            encodeRegistration form |> Http.jsonBody

        request =
            Http.post registerUrl body profileDecoder
    in
        Http.send msg request


providers : List Provider
providers =
    []


provider : Id -> Maybe Provider
provider id =
    Nothing


links : Id -> Links
links profileId =
    initLinks


addLink : Id -> Link -> Result String Links
addLink profileId link =
    Err "Not implemented"


removeLink : Id -> Link -> Result String Links
removeLink profileId link =
    Err "Not implemented"


topicLinks : Topic -> ContentType -> Id -> List Link
topicLinks topic contentType id =
    []


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


subscriptions : Id -> Subscribers
subscriptions profileId =
    Subscribers []


followers : Id -> Subscribers
followers profileId =
    Subscribers []


follow : Id -> Id -> Result String ()
follow clientId providerId =
    Err "follow not implemented"


unsubscribe : Id -> Id -> Result String ()
unsubscribe clientId providerId =
    Err "unsubscribe not implemented"
