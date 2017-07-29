module Nikeza.Server.YouTube

open System
open Nikeza.YouTube.Data

let getVideos youtube parameters = 
    async {
        let!   videos = uploadList youtube parameters
        let    out =    videos |> Seq.map(fun video -> sprintf "Title: %s\nVideoId: %s\n" video.title video.videoId)
                               |> Seq.reduce(+)
        return out
    }   |> Async.RunSynchronously