module Services.Gateway exposing (..)

import Controls.Login as Login exposing (Model)
import Domain.Core exposing (..)
import Http exposing (getString)
import Json.Decode as Decode exposing (Decoder, field)
import Json.Encode as Encode


-- DECODERS/ENCODERS


decoder : Decoder JsonProfile
decoder =
    Decode.map4 JsonProfile
        (field "Id" Decode.string)
        (field "FirstName" Decode.string)
        (field "LastName" Decode.string)
        (field "Email" Decode.string)


encode : Form -> Encode.Value
encode form =
    Encode.object
        [ ( "FirstName", Encode.string form.firstName )
        , ( "LastName", Encode.string form.firstName )
        , ( "Email", Encode.string form.email )
        , ( "Password", Encode.string form.password )
        ]


tryLogin : Login.Model -> Login.Model
tryLogin credentials =
    let
        successful =
            String.toLower credentials.email == "test" && String.toLower credentials.password == "test"
    in
        if successful then
            { email = credentials.email, password = credentials.password, loggedIn = True }
        else
            { email = credentials.email, password = credentials.password, loggedIn = False }


tryRegister : Form -> (Result Http.Error JsonProfile -> msg) -> Cmd msg
tryRegister form msg =
    let
        registerUrl =
            "http://localhost:5000/register"

        body =
            encode form |> Http.jsonBody

        request =
            Http.post registerUrl body decoder
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
