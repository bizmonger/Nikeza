module Events

open Nikeza.Common
open Nikeza.DataTransfer

type Events =
    | LinkFeatured         of LinkId
    | LinkFeaturedFailed   of LinkId

    | LinkUnfeatured       of LinkId
    | LinkUnfeaturedFailed of LinkId

    | TopicsFeatured       of TopicId list
    | TopicsFeaturedFailed of TopicId list

    | PortfolioReturned    of Provider
    | PortfolioFailed      of ProviderId