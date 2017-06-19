module Domain.ContributorPortal exposing (..)

import Domain.Core exposing (..)
import Domain.Contributor as Contributor exposing (..)


-- MODEL


type alias Model =
    { contributor : Contributor
    , requested : ContributorRequest
    , newConnection : Connection
    , newLinks : NewLinks
    }


init : Model
init =
    { contributor = Contributor.init
    , requested = ViewLinks
    , newConnection = initConnection
    , newLinks = initNewLinks
    }
