module Nikeza.Server.Http

open System.Net.Http
open System.Net.Http.Headers
open System
open System.Net
open Asynctify

let httpClient baseAddress =

    System.Diagnostics.Debug.WriteLine(baseAddress)
    let handler = new HttpClientHandler()
    handler.AutomaticDecompression <- DecompressionMethods.GZip ||| DecompressionMethods.Deflate

    let client = new HttpClient(handler)
    client.BaseAddress <- Uri(baseAddress)
    client.DefaultRequestHeaders.Accept.Clear()
    client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue("application/json"))
    client

let sendRequest baseAddress (url:string) accessId key =
    use client =   httpClient baseAddress
    let url =      String.Format(url, accessId, key)
    let response = client.GetAsync(url) |> toResult
    response