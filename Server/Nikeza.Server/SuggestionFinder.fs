module Nikeza.Server.SuggestionFinder

let getAllTags (searchItem:string) (tags: string list) =
    if searchItem <> "" && searchItem.Length > 1
        then tags |> List.map (fun t -> t.ToLower())
        else []

let findMatch (text:string) tags =

    let tags = tags |> getAllTags text

    if not (tags |> List.isEmpty)
       then tags 
             |> List.filter(fun t -> t.Contains(text.ToLower()))
             |> List.filter (fun t -> t = text)
             |> List.tryHead
       else None

let getSuggestions (text:string) tags =

    if not (tags |> List.isEmpty)
       then tags |> findMatch text
                 |> function
                    | Some tag -> [tag]
                    | None     -> tags |> List.filter(fun t -> t.Contains(text.ToLower()))
        else []