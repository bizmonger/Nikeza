// This is a demo cli 

open System
open VideoYouTube.Data

type VideoRequestKind = 
    | ForUserName
    | ChannelId

let getVideos youtube kind = 
    let params = 
        match kind with 
        | ForUserName -> 
            printf "Enter YouTube user name: "
            let userName = Console.ReadLine()
            { id = None; userName = Some(userName) }
        | ChannelId ->
            printf "Enter YouTube Channel Id: "
            let id = Console.ReadLine()
            { id = Some(id); userName = None }
    async {
        let! videos = uploadList youtube params
        let out = 
            videos 
            |> Seq.map(fun video -> sprintf "Title: %s\nVideoId: %s\n" video.title video.videoId)
            |> Seq.reduce(+)
        return out
    } 
    |> Async.RunSynchronously
    |> printfn "%s"

[<EntryPoint>]
let main argv =
    printf "Enter API Key: "
    let key = Console.ReadLine()
    let apiKey = key
    let youtube = youTubeService apiKey
    let downloader = getVideos youtube 
    downloader ChannelId
    downloader ForUserName

    printfn "Done"
    0