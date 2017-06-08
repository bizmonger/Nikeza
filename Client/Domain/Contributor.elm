module Domain.Contributor exposing (..)

import Domain.Core exposing (..)


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
        Model profile [] [] [] []


type alias Model =
    { profile : Profile
    , topics : List Topic
    , articles : List Link
    , videos : List Link
    , podcasts : List Link
    }
