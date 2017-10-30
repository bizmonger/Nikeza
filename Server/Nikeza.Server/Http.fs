module Nikeza.Server.Http

open System.Net.Http
open System.Net.Http.Headers
open System
open System.Net

let httpClient baseAddress =

    let handler = new HttpClientHandler()
    handler.AutomaticDecompression <- DecompressionMethods.GZip ||| DecompressionMethods.Deflate

    let client = new HttpClient(handler)
    client.BaseAddress <- Uri(baseAddress)
    client.DefaultRequestHeaders.Accept.Clear()
    client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue("application/json"))
    client