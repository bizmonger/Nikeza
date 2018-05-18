module Nikeza.Mobile.UILogic.Helpers

open Nikeza.Common
open Nikeza.DataTransfer

type Adapter = { 
    Profile       : Profile
    RecentLinks   : Link list
}

let toSubscriptions (result:ProviderRequest list) =

    let toAdapters (subscription:ProviderRequest) =

        { Profile =    subscription.Profile
          RecentLinks= subscription.RecentLinks
        }
                            
    let rec completeSet(adapters) = 

        let maxPlaceholders = 3

        if   List.length adapters < maxPlaceholders

        then let updatedAdapters = { adapters.Head with Title = "" } :: adapters
             completeSet(updatedAdapters)

        else adapters
        
    result 
     |> List.map toAdapters 
     |> List.map ( fun adapter -> { adapter with RecentLinks= adapter.RecentLinks |> completeSet } )