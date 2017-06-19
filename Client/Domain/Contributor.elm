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
        Contributor initProfile True initTopics initLinks
