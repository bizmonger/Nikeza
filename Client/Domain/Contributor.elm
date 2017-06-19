module Domain.Contributor exposing (..)

import Domain.Core exposing (..)
import Settings exposing (..)


type alias Model =
    Contributor


init : Model
init =
    let
        addedLinks =
            NewLinks initLinkToCreate False []
    in
        Contributor initProfile initConnection addedLinks True initTopics initLinks


getContributor : Profile -> Model
getContributor p =
    { profile = p
    , showAll = True
    , topics = p.topics
    , newConnection = initConnection
    , newLinks = initNewLinks
    , links = p.id |> runtime.links
    }
