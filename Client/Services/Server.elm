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


latestPosts : Id -> ContentType -> List Post
latestPosts id contentType =
    []


recentContributors : List Profile
recentContributors =
    []


getContributor : Id -> Maybe Profile
getContributor id =
    Nothing


posts : ContentType -> Id -> List Post
posts profileId contentType =
    []


topicPosts : Topic -> ContentType -> Id -> List Post
topicPosts topic contentType id =
    []
