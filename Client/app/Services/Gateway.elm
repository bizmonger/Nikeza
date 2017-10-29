module Services.Gateway exposing (..)

import Domain.Core exposing (..)
import Services.Adapter exposing (..)
import Services.Encoders exposing (..)
import Services.Decoders exposing (..)
import String exposing (..)
import Http exposing (getString, Request, expectStringResponse, header)
import Json.Decode as Decode exposing (Decoder, field)
import Json.Encode as Encode


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


recentLinks : Id -> (Result Http.Error (List JsonLink) -> msg) -> Cmd msg
recentLinks profileId msg =
    let
        url =
            baseUrl ++ "recent"

        request =
            Http.get url (Decode.list linkDecoder)
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


portfolio : Id -> (Result Http.Error JsonPortfolio -> msg) -> Cmd msg
portfolio profileId msg =
    let
        url =
            baseUrl ++ (idText profileId) ++ "links"

        body =
            (encodeId profileId) |> Http.jsonBody

        request =
            Http.post url body portfolioDecoder
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


addLink : Link -> (Result Http.Error JsonLink -> msg) -> Cmd msg
addLink link msg =
    let
        url =
            baseUrl ++ "addlink"

        body =
            encodeLink link |> Http.jsonBody

        request =
            Http.post url body linkDecoder
    in
        Http.send msg request


removeLink : Link -> (Result Http.Error JsonLink -> msg) -> Cmd msg
removeLink link msg =
    let
        url =
            baseUrl ++ "removelink"

        body =
            encodeLink link |> Http.jsonBody

        request =
            Http.post url body linkDecoder
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


suggestedTopics : String -> (Result Http.Error (List String) -> msg) -> Cmd msg
suggestedTopics search msg =
    let
        url =
            baseUrl ++ "suggestedtopics"

        request =
            Http.get url (Decode.list Decode.string)
    in
        Http.send msg request


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
