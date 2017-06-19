module Domain.ContributorPortal exposing (..)

import Domain.Core exposing (..)
import Domain.Contributor as Contributor exposing (..)


-- MODEL


type alias Model =
    ContributorPortal


init : Model
init =
    { contributor = Contributor.init
    , requested = ViewLinks
    , newConnection = initConnection
    , newLinks = initNewLinks
    }
