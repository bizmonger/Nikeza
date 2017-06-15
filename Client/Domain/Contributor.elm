module Domain.Contributor exposing (..)

import Domain.Core exposing (..)
import Controls.AddConnection as AddConnection exposing (..)
import Controls.NewLinks as NewLinks exposing (..)
import Settings exposing (..)


init : Model
init =
    let
        addedLinks =
            NewLinks initLinkToCreate False []
    in
        Model initProfile initConnection addedLinks True [] [] [] [] []


type alias Model =
    { profile : Profile
    , newConnection : AddConnection.Model
    , newLinks : NewLinks.Model
    , showAll : Bool
    , topics : List Topic
    , answers : List Link
    , articles : List Link
    , videos : List Link
    , podcasts : List Link
    }


getContributor : Profile -> Model
getContributor p =
    { profile = p
    , showAll = True
    , topics = p.topics
    , newConnection = initConnection
    , newLinks = NewLinks.init
    , answers = p.id |> runtime.links Answer
    , articles = p.id |> runtime.links Article
    , videos = p.id |> runtime.links Video
    , podcasts = p.id |> runtime.links Podcast
    }
