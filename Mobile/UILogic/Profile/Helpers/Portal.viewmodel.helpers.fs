module Nikeza.Mobile.UILogic.Helpers

open Nikeza.Common

[<AllowNullLiteralAttribute>]
type Adapter(profile,links) =
    member x.Profile     = profile
    member x.RecentLinks = links

let toSubscriptions (result:ProviderRequest list) =

    let toAdapters (subscription:ProviderRequest) =

        Adapter(subscription.Profile, subscription.RecentLinks)
                            
    let rec completeSet(adapters) = 

        let maxPlaceholders = 3

        if   List.length adapters < maxPlaceholders

        then let updatedAdapters = { adapters.Head with Title = "" } :: adapters
             completeSet(updatedAdapters)

        else adapters
        
    result 
     |> List.map toAdapters 
     |> List.map ( fun adapter -> Adapter(adapter.Profile, completeSet adapter.RecentLinks) )//{ adapter with RecentLinks= adapter.RecentLinks |> completeSet } )
     //|> List.map ( fun adapter -> { adapter with RecentLinks= adapter.RecentLinks |> completeSet } )