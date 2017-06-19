module Domain.Portal exposing (..)

import Domain.Core exposing (..)
import Domain.Contributor as Contributor exposing (..)


-- MODEL


type alias Model =
    ContributorPortal


init : Model
init =
    { contributor = initContributor
    , requested = ViewLinks
    , newConnection = initConnection
    , newLinks = initNewLinks
    }
