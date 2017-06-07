module Domain.Contributor exposing (..)

import Domain.Core exposing (..)


model : Model
model =
    let
        profile =
            { id = Id undefined
            , name = Contributor undefined
            , imageUrl = Url undefined
            , bio = undefined
            , topics = []
            }
    in
        Model False profile [] [] [] []


type alias Model =
    { topicSelected : Bool
    , profile : Profile
    , topics : List Topic
    , articles : List Post
    , videos : List Post
    , podcasts : List Post
    }
