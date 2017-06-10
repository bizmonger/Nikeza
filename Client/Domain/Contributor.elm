module Domain.Contributor exposing (..)

import Domain.Core exposing (..)
import Settings exposing (..)


init : Model
init =
    let
        profile =
            { id = Id undefined
            , name = Contributor undefined
            , imageUrl = Url undefined
            , bio = undefined
            , topics = []
            }
    in
        Model profile [] [] [] [] []


type alias Model =
    { profile : Profile
    , topics : List Topic
    , answers : List Link
    , articles : List Link
    , videos : List Link
    , podcasts : List Link
    }


getContributor : Profile -> Model
getContributor p =
    { profile = p
    , topics = p.topics
    , answers = p.id |> runtime.links Answer
    , articles = p.id |> runtime.links Article
    , videos = p.id |> runtime.links Video
    , podcasts = p.id |> runtime.links Podcast
    }
