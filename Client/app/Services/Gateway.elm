module Services.Gateway exposing (..)

import Domain.Core exposing (..)
import Services.Adapter exposing (..)
import Services.Encoders exposing (..)
import Services.Decoders exposing (..)
import String exposing (..)
import Http exposing (getString, Request, expectStringResponse, header)
import Json.Decode as Decode exposing (Decoder, field)


baseUrl : String
baseUrl =
    "http://nikeza.io/"



--"http://localhost:5000/"


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


updateThumbnail : UpdateThumbnailRequest -> (Result Http.Error String -> msg) -> Cmd msg
updateThumbnail updateThumbnailRequest msg =
    let
        url =
            baseUrl ++ "updatethumbnail"

        body =
            encodeThumbnailUpdate updateThumbnailRequest |> Http.jsonBody

        request =
            Http.post url body Decode.string
    in
        Http.send msg request


thumbnail : Platform -> String -> (Result Http.Error JsonThumbnail -> msg) -> Cmd msg
thumbnail platform accessId msg =
    let
        url =
            baseUrl ++ "thumbnail/" ++ (platform |> platformText |> String.toLower) ++ "/" ++ accessId

        request =
            Http.get url thumbnailDecoder
    in
        Http.send msg request


recentLinkProviders : Id -> (Result Http.Error (List JsonProvider) -> msg) -> Cmd msg
recentLinkProviders subscriberId msg =
    let
        url =
            baseUrl ++ "recent/" ++ (idText subscriberId)

        request =
            Http.get url (Decode.list providerDecoder)
    in
        Http.send msg request


providers : (Result Http.Error (List JsonProvider) -> msg) -> Cmd msg
providers msg =
    let
        url =
            baseUrl ++ "providers/wtf"

        request =
            Http.get url (Decode.list providerDecoder)
    in
        Http.send msg request


provider : Id -> (Result Http.Error JsonProvider -> msg) -> Cmd msg
provider id msg =
    let
        url =
            baseUrl ++ "provider/" ++ (idText id)

        request =
            Http.get url providerDecoder
    in
        Http.send msg request


featuredTopics : Id -> List String -> (Result Http.Error JsonProvider -> msg) -> Cmd msg
featuredTopics id topics msg =
    let
        url =
            baseUrl ++ "featuredtopics"

        body =
            (encodeFeaturedTopics id topics) |> Http.jsonBody

        request =
            Http.post url body providerDecoder
    in
        Http.send msg request


links : Id -> (Result Http.Error JsonProvider -> msg) -> Cmd msg
links profileId msg =
    let
        url =
            baseUrl ++ "links/" ++ (idText profileId)

        request =
            Http.get url providerDecoder
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


featureLink : FeatureLink -> (Result Http.Error Int -> msg) -> Cmd msg
featureLink toggleRequest msg =
    let
        url =
            baseUrl ++ "featurelink"

        body =
            encodeFeatureLink toggleRequest |> Http.jsonBody

        request =
            Http.post url body Decode.int
    in
        Http.send msg request


sources : Id -> (Result Http.Error (List JsonSource) -> msg) -> Cmd msg
sources profileId msg =
    let
        url =
            baseUrl ++ "sources/" ++ idText profileId

        request =
            Http.get url (Decode.list sourceDecoder)
    in
        Http.send msg request


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


removeSource : Id -> (Result Http.Error String -> msg) -> Cmd msg
removeSource sourceId msg =
    case (idText sourceId) |> String.toInt of
        Ok id ->
            let
                url =
                    baseUrl ++ "removesource/" ++ (id |> toString)

                request =
                    Http.get url Decode.string
            in
                Http.send msg request

        Err _ ->
            Cmd.none


bootstrap : (Result Http.Error JsonBootstrap -> msg) -> Cmd msg
bootstrap msg =
    let
        url =
            baseUrl ++ "bootstrap/wtf"

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


updateProfileAndTopics : ProfileAndTopics -> (Result Http.Error JsonProvider -> msg) -> Cmd msg
updateProfileAndTopics profileAndTopics msg =
    let
        url =
            baseUrl ++ "updateprovider"

        body =
            encodeProfileAndTopics profileAndTopics |> Http.jsonBody

        request =
            Http.post url body providerDecoder
    in
        Http.send msg request


suggestedTopics : String -> (Result Http.Error (List String) -> msg) -> Cmd msg
suggestedTopics search msg =
    if not <| isEmpty search && String.length search > 1 then
        let
            url =
                baseUrl ++ "suggestedtopics/" ++ (Http.encodeUri search)

            request =
                Http.get url (Decode.list Decode.string)
        in
            Http.send msg request
    else
        Cmd.none


subscriptions : Id -> (Result Http.Error (List JsonProvider) -> msg) -> Cmd msg
subscriptions profileId msg =
    let
        url =
            baseUrl ++ "subscriptions/" ++ idText profileId

        request =
            Http.get url (Decode.list providerDecoder)
    in
        Http.send msg request


followers : Id -> (Result Http.Error (List JsonProvider) -> msg) -> Cmd msg
followers profileId msg =
    let
        url =
            baseUrl ++ "followers/" ++ idText profileId

        request =
            Http.get url (Decode.list providerDecoder)
    in
        Http.send msg request


follow : SubscriptionRequest -> (Result Http.Error JsonSubscriptionActionResponse -> msg) -> Cmd msg
follow followRequest msg =
    let
        url =
            baseUrl ++ "follow"

        body =
            encodeSubscriptionRequest followRequest |> Http.jsonBody

        request =
            Http.post url body subscriptionActionResponseDecoder
    in
        Http.send msg request


unsubscribe : SubscriptionRequest -> (Result Http.Error JsonSubscriptionActionResponse -> msg) -> Cmd msg
unsubscribe unsubscribeRequest msg =
    let
        url =
            baseUrl ++ "unsubscribe"

        body =
            encodeSubscriptionRequest unsubscribeRequest |> Http.jsonBody

        request =
            Http.post url body subscriptionActionResponseDecoder
    in
        Http.send msg request
