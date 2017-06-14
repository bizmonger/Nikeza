module Domain.Contributor exposing (..)

import Domain.Core exposing (..)
import Controls.AddConnection as AddConnection exposing (..)
import Controls.AddLink as AddLink exposing (..)
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

        newLink =
            { profile = profile
            , title = Title undefined
            , url = Url undefined
            , topics = []
            }
    in
        Model profile newConnection ( newLink, False ) True [] [] [] [] []


type alias Model =
    { profile : Profile
    , newConnection : AddConnection.Model
    , newLink : ( AddLink.Model, Bool )
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
    , newLink = ( { profile = p, title = Title "undefined", url = Url "undefined", topics = [] }, False )
    , answers = p.id |> runtime.links Answer
    , articles = p.id |> runtime.links Article
    , videos = p.id |> runtime.links Video
    , podcasts = p.id |> runtime.links Podcast
    }
