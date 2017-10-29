module Nikeza.Server.Http

open System.Net.Http
open System.Net.Http.Headers
open System

let httpClient baseAddress =
    let client = new HttpClient()
    client.BaseAddress <- Uri(baseAddress)
    client.DefaultRequestHeaders.Accept.Clear()
    client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue("application/json"))
    client