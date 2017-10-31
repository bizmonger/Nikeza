namespace Nikeza.Server

module StackOverflow =
    let getThumbnail accessId apiKey = ""

    module Tags =

        open System
        open System.IO
        open Newtonsoft.Json
        open Nikeza.Server.Http
        open Nikeza.Server.Literals

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
            
            let client = httpClient APIBaseAddress

            try
                let url =      String.Format(TagsUrl, pageNumber |> string)
                let urlWithKey = sprintf "%s&key=%s" url (File.ReadAllText(KeyFile_StackOverflow))
                let response = client.GetAsync(urlWithKey) |> Async.AwaitTask 
                                                           |> Async.RunSynchronously

                if response.IsSuccessStatusCode
                then let json =   response.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
                     let result = JsonConvert.DeserializeObject<Response>(json)
                     let tags =   result.items |> List.ofSeq 
                                               |> List.map (fun i -> i.name)
                     tags
                else []

            finally  client.Dispose()

    open Tags

    module CachedTags =
        let private x = [1..25] |> List.collect (fun page -> getTags(page))
        let Instance() = x

    module Suggestions =
        
        open Nikeza.Server.Http

        let getRelatedTags (tag:string) =

            if tag <> ""
            then let parseTag (text:string) =
                     let index = text.IndexOf("|")
                     if  index > 0
                         then Some <| text.Substring(0,index)
                         else None
                 
                 let client = httpClient SiteBaseAddress
         
                 try let relatedTagsUrl = sprintf "filter/tags?q=%s" tag
                     let response =       client.GetAsync(relatedTagsUrl) |> Async.AwaitTask 
                                                                          |> Async.RunSynchronously
                     if response.IsSuccessStatusCode
                     then let result = response.Content.ReadAsStringAsync() |> Async.AwaitTask 
                                                                            |> Async.RunSynchronously 
                          result.Split('\n') |> List.ofArray 
                                             |> List.filter (fun x -> x <> "")
                                             |> List.tryHead
                                             |> function 
                                                | None -> []
                                                | Some formatted ->
                                                    let tags = formatted.Split("\\n") 
                                                               |> List.ofArray
                                                               |> List.choose parseTag
                                                               |> List.map   (fun tag -> tag.Replace(@"""", ""))
                                                               |> List.filter(fun current -> current <> tag)
                                                    tag::tags
                     else []
     
                 finally client.Dispose()
            else []
                
        let getSuggestions (searchItem:string) =
            if searchItem <> ""
            then let tags =         CachedTags.Instance() |> List.map (fun t -> t.ToLower())
                 let filteredTags = tags |> List.filter(fun t -> t.Contains(searchItem.ToLower()))
                 let matchingTags = filteredTags |> List.filter (fun t -> t = searchItem)

                 if matchingTags |> List.isEmpty |> not
                     then getRelatedTags matchingTags.Head
                     else filteredTags
            else []