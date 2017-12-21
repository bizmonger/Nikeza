module Nikeza.Mobile.Commands

open Nikeza.Common

type Command =
    | FeatureLink   of LinkId
    | UnfeatureLink of LinkId

    | View          of ProviderRequest