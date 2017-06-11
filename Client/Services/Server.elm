module Services.Server exposing (..)

import Controls.Login as Login exposing (Model)
import Domain.Core exposing (..)


tryLogin : Login.Model -> Login.Model
tryLogin credentials =
    -- TODO - Replace tbelow his with integration code...
    let
        successful =
            String.toLower credentials.username == "test" && String.toLower credentials.password == "test"
    in
        if successful then
            { username = credentials.username, password = credentials.password, loggedIn = True }
        else
            { username = credentials.username, password = credentials.password, loggedIn = False }


latestLinks : Id -> ContentType -> List Link
latestLinks id contentType =
    []


contributors : List Profile
contributors =
    []


contributor : Id -> Maybe Profile
contributor id =
    Nothing


links : ContentType -> Id -> List Link
links profileId contentType =
    []


topicLinks : Topic -> ContentType -> Id -> List Link
topicLinks topic contentType id =
    []


usernameToId : String -> Id
usernameToId username =
    Id "undefined"
