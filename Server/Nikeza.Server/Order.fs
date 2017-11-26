module Nikeza.Server.Order

open DatabaseCommand
open Model
open Commands

    let execute = function
        | UpdateProfile   info -> updateProfile    info
        | UpdateThumbnail info -> updateThumbnail  info
       
        | Follow          info -> follow           info
        | Unsubscribe     info -> unsubscribe      info
      
        | AddLink         info -> addLink          info
        | RemoveLink      info -> removeLink       info
        | FeatureLink     info -> featureLink      info
        | ObserveLinks    info -> observeLinks     info

        | UpdateTopics    info -> featureTopics    info

        | AddSource       info -> addDataSource    info
        | RemoveSource    info -> removeDataSource info