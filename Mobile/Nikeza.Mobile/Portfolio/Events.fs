module Nikeza.Mobile.Portfolio.Events

open Nikeza.Common

type LinksEvent =
    | LinkFeatured         of LinkId
    | LinkFeaturedFailed   of LinkId

    | LinkUnfeatured       of LinkId
    | LinkUnfeaturedFailed of LinkId

type TopicsEvent =
    | TopicsFeatured       of TopicId list
    | TopicsFeaturedFailed of TopicId list