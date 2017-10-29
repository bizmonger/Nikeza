module Nikeza.Server.StackOverflow

open Nikeza.Server.Http

(* Get most popular tags (100 / page) *)
// https://api.stackexchange.com/docs/tags#page=5&pagesize=100&order=desc&sort=popular&filter=!-.G.68grSaJm&site=stackoverflow&run=true

(* Answers *)
// https://api.stackexchange.com/2.2/users/492701/answers?order=desc&sort=activity&site=stackoverflow&filter=!Fcazzsr2b3M)LbUjGAu-Fs0Wf8

(* Related Tags *)
// https://stackoverflow.com/filter/tags?q=xamarin


[<Literal>]
let BaseAddress = "https://stackoverflow.com/"
let getTags pages = []

let getSuggestions searchItem =

    let url =      sprintf "filter/tags?q=%s" searchItem
    let client =   httpClient BaseAddress
    let response = client.GetAsync(url) |> Async.AwaitTask 
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