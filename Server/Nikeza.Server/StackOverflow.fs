namespace Nikeza.Server.StackOverflow

module Tags =

    open System
    open Nikeza.Server.Http
    open Newtonsoft.Json
    open System.Text

    (* Answers *)
    // https://api.stackexchange.com/2.2/users/492701/answers?order=desc&sort=activity&site=stackoverflow&filter=!Fcazzsr2b3M)LbUjGAu-Fs0Wf8

    [<Literal>]
    let TagsUrl = "2.2/tags?page={0}&order=desc&sort=popular&site=stackoverflow&filter=!-.G.68grSaJm"

    [<Literal>]
    let APIBaseAddress = "https://api.stackexchange.com/"

    [<Literal>]
    let SiteBaseAddress = "https://stackoverflow.com/"

    type Item =     { name : string }
    type Response = { items: Item list }

    let getTags (pageNumber:int) : string list =

        let url =      String.Format(TagsUrl, pageNumber |> string)
        let client =   httpClient APIBaseAddress
        let response = client.GetAsync(url) |> Async.AwaitTask 
                                            |> Async.RunSynchronously

        if response.IsSuccessStatusCode
        then let json =   response.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
             let result = JsonConvert.DeserializeObject<Response>(json)
             let tags =   result.items |> List.ofSeq 
                                       |> List.map (fun i -> i.name)
             tags
        else []

open Tags

module CachedTags =
    let private x = 
        Lazy.Create(fun() -> [1..25] |> List.collect (fun page -> getTags(page)))
    let Instance() = x.Value

module Suggestions =
    
    open Nikeza.Server.Http

    let getRelatedTags (tag:string) =

        let parseTag (text:string) =
            let index = text.IndexOf("|")
            if  index > 0
                then Some <| text.Substring(0,index)
                else None
    
        let relatedTagsUrl = sprintf "filter/tags?q=%s" tag
        let client =         httpClient SiteBaseAddress
        let response =       client.GetAsync(relatedTagsUrl) |> Async.AwaitTask 
                                                             |> Async.RunSynchronously
        if response.IsSuccessStatusCode
            then let result = response.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously 
                 let parts =       result.Split('\n') |> List.ofArray |> List.filter (fun x -> x <> "")
                 parts |> List.tryHead
                 |> function 
                    | Some formatted ->
                        let tags = formatted.Split("\\n") 
                                   |> List.ofArray
                                   |> List.choose parseTag
                                   |> List.map (fun tag -> tag.Replace(@"""", ""))
                        tags
                    | None -> []
            else []
            
    let getSuggestions (searchItem:string) =

        let tags =         CachedTags.Instance() |> List.map (fun t -> t.ToLower())
        let filteredTags = tags |> List.filter(fun t -> t.Contains(searchItem.ToLower()))
        let matchingTags = filteredTags |> List.filter (fun t -> t = searchItem)

        if matchingTags |> List.isEmpty |> not
            then getRelatedTags matchingTags.Head
            else filteredTags