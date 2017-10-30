namespace Nikeza.Server.StackOverflow

module Tags =
    open System
    open Nikeza.Server.Http
    open Newtonsoft.Json

    (* Get most popular tags (100 / page) *)
    // https://api.stackexchange.com/2.2/tags?page=1&order=desc&sort=popular&site=stackoverflow&filter=!-.G.68grSaJm

    (* Answers *)
    // https://api.stackexchange.com/2.2/users/492701/answers?order=desc&sort=activity&site=stackoverflow&filter=!Fcazzsr2b3M)LbUjGAu-Fs0Wf8

    (* Related Tags *)
    // https://stackoverflow.com/filter/tags?q=xamarin

    [<Literal>]
    let TagsUrl = "docs/tags#page={0}&pagesize=100&order=desc&sort=popular&filter=!-.G.68grSaJm&site=stackoverflow&run=true"

    [<Literal>]
    let APIBaseAddress = "https://api.stackexchange.com/"

    [<Literal>]
    let SiteBaseAddress = "https://stackoverflow.com/"

    type Item =     { name : string }
    type Response = { items: Item list }

    let getTags (pageNumber:int) =

        let tagsUrl =  String.Format(TagsUrl, pageNumber |> string)
        let client =   httpClient APIBaseAddress
        let response = client.GetAsync(tagsUrl) |> Async.AwaitTask 
                                                |> Async.RunSynchronously
        if response.IsSuccessStatusCode
            then let json =   response.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
                 let result = JsonConvert.DeserializeObject<Response>(json)
                 let tags =   result.items |> List.ofSeq 
                                           |> List.map (fun i -> i.name)
                 tags
            else []

open Tags

type Cache() =
    let mutable tags = []
    member this.GetTags() = 
        if tags |> List.isEmpty
            then tags <- [1..25] |> List.collect (fun page -> getTags(page))
                 tags
            else tags

module Suggestions =
    
    open Nikeza.Server.Http

    let getRelatedTags (tag:string) =
    
        let relatedTagsUrl = sprintf "filter/tags?q=%s" tag
        let client =         httpClient SiteBaseAddress
        let response =       client.GetAsync(relatedTagsUrl) |> Async.AwaitTask 
                                                             |> Async.RunSynchronously
        if response.IsSuccessStatusCode
            then let result = response.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously 
                 let formattedResult = "\n" + result
                 let topics =  formattedResult.Split('\n') 
                               |> List.ofArray 
                               |> List.filter (fun i -> i <> "")
                               |> List.choose(fun p -> let index = p.IndexOf("|")
                                                       if  index > 0
                                                           then Some <| p.Substring(0,index)
                                                           else None
                                              )
                 topics
            else []
            
    let getSuggestions (searchItem:string) =

        let tags =         Cache().GetTags() |> List.map (fun t -> t.ToLower())
        let filteredTags = tags |> List.filter(fun t -> t.Contains(searchItem.ToLower()))
        let matchingTags = filteredTags |> List.filter (fun t -> t = searchItem)

        if matchingTags |> List.isEmpty |> not
            then getRelatedTags matchingTags.Head
            else matchingTags