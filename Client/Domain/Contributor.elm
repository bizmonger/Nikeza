module Domain.Contributor exposing (..)

import Domain.Core exposing (..)
import Controls.AddConnection as AddConnection exposing (..)
import Settings exposing (..)


init : Model
init =
    let
        profile =
            { id = Id undefined
            , name = Name undefined
            , imageUrl = Url undefined
            , bio = undefined
            , topics = []
            , connections = []
            }

        newConnection =
            { platform = "", username = "" }
    in
        Model profile newConnection True [] [] [] [] []


type alias Model =
    { profile : Profile
    , newConnection : AddConnection.Model
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
    , newConnection = AddConnection.init
    , answers = p.id |> runtime.links Answer
    , articles = p.id |> runtime.links Article
    , videos = p.id |> runtime.links Video
    , podcasts = p.id |> runtime.links Podcast
    }
