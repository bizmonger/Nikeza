module Nikeza.Server.Command

open DatabaseCommand
open Model
open Commands

let execute = function
    | UpdateProfile   info -> info |> updateProfile
    | UpdateThumbnail info -> info |> updateThumbnail

    | Follow          info -> info |> follow
    | Unsubscribe     info -> info |> unsubscribe

    | AddLink         info -> info |> addLink
    | RemoveLink      info -> info |> removeLink 
    | FeatureLink     info -> info |> featureLink
    | ObserveLinks    info -> info |> observeLinks

    | UpdateTopics    info -> info |> featureTopics

    | AddSource       info -> info |> addDataSource
    | RemoveSource    info -> info |> removeDataSource
    | SyncSource      info -> info |> syncDataSource