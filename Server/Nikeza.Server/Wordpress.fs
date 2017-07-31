module Nikeza.Server.Wordpress

open System
open System.Text
open System.Net.Http
open Nikeza.Wordpress.Rss.Data
open Newtonsoft.Json

let jsonRssFeed (feedUrl: string) = async { 
        let  client = new  HttpClient()
        let! response =    client.GetByteArrayAsync(feedUrl) |> Async.AwaitTask   
        let  feed =        Encoding.UTF8.GetString(response,0, (response.Length - 1)) |> deserializeXml
        let  json =        JsonConvert.SerializeObject(feed)
        return json
}