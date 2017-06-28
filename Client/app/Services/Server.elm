module Services.Server exposing (..)

import Controls.Login as Login exposing (Model)
import Controls.Register as Register exposing (Model)
import Domain.Core exposing (..)


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


tryRegister : Register.Model -> Result String ContentProvider
tryRegister form =
    Err "Registration failed"


contentProviders : List ContentProvider
contentProviders =
    []


contentProvider : Id -> Maybe ContentProvider
contentProvider id =
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


connections : Id -> List Source
connections profileId =
    []


addSource : Id -> Source -> Result String (List Source)
addSource profileId connection =
    Err "Not implemented"


removeSource : Id -> Source -> Result String (List Source)
removeSource profileId connection =
    Err "Not implemented"


topics : List Topic
topics =
    []


platforms : List Platform
platforms =
    []


suggestedTopics : String -> List Topic
suggestedTopics search =
    []
