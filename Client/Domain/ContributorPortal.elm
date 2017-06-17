module Domain.ContributorPortal exposing (..)

import Domain.Core exposing (..)
import Domain.Contributor as Contributor exposing (..)


-- MODEL


type alias Model =
    { contributor : Contributor.Model, requested : ContributorRequest }


init : Model
init =
    { contributor = Contributor.init
    , requested = CurrentLinks
    }
