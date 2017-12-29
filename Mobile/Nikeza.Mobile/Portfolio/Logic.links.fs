module internal LinkResult

open Events
open Commands
open Commands.ResultOf

type private Handle = ResultOf.Link -> LinksEvent list

let handle : Handle =
    fun response ->
        response |> function
                    | Link.Feature   result -> 
                                     result |> function
                                               | Ok    linkId -> [LinkFeatured       linkId]
                                               | Error linkId -> [LinkFeaturedFailed linkId]
                    | Link.Unfeature result -> 
                                     result |> function
                                               | Ok    linkId -> [LinkUnfeatured       linkId]
                                               | Error linkId -> [LinkUnfeaturedFailed linkId]