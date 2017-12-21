module Nikeza.Mobile.Events

open Nikeza.Common

type LinkEvents =
    | LinkFeatured   of FeatureLinkRequest
    | LinkUnfeatured of FeatureLinkRequest

type TopicEvents = TopicsFeatured of FeatureTopicsrequest