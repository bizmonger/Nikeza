module Events

open Nikeza.Common
open Nikeza.DataTransfer

type LinksEvent =
    | LinkFeatured         of LinkId
    | LinkFeaturedFailed   of LinkId

    | LinkUnfeatured       of LinkId
    | LinkUnfeaturedFailed of LinkId

type TopicsEvent =
    | TopicsFeatured       of TopicId list
    | TopicsFeaturedFailed of TopicId list

type PortfolioEvent =
    | PortfolioReturned    of Provider
    | PortfolioFailed      of ProviderId