module Nikeza.Server.YouTube

open System
open FSharp.Control
open Google.Apis.Services
open Google.Apis.YouTube.v3
open Google.Apis.YouTube.v3.Data

[<Literal>]
let UrlPrefix = "https://www.youtube.com/watch?v="

type Video = { title: string; videoId: string }

type Content =  Details | Snippet

type Id = 
    | UserName  of string
    | ChannelId of string

let ContentStr = function
    | Details -> "ContentDetails"
    | Snippet -> "Snippet"

module Authentication = 
    [<Literal>]    
    let private AppName = "NIEKEZA-video-youtube"

    let youTubeService apiKey =
        new YouTubeService(
            BaseClientService.Initializer(
                ApiKey = apiKey,
                ApplicationName = AppName
            )
        )

    // TODO OAuth

module Channel = 

    type private Request = ChannelsResource.ListRequest

    let private setupRequest (req: Request) id = 
        match id with
        | UserName  username  -> do req.ForUsername <- username
        | ChannelId channelId -> do req.Id          <- channelId
        req
    
    let private execute (req: Request) = req.ExecuteAsync() |> Async.AwaitTask

    type private RequestChannelById = YouTubeService -> Id -> Async<ChannelListResponse>
    let list: RequestChannelById = fun youTubeService id ->
        youTubeService.Channels.List(ContentStr(Details))
        |> setupRequest <| id
        |> execute

module Playlist = 

    type private ConfigPlaylistReq = string -> PlaylistItemsResource.ListRequest -> string -> PlaylistItemsResource.ListRequest

    let private configPlaylistReq: ConfigPlaylistReq = fun playListId request nextPageToken ->
        do request.PlaylistId <- playListId
           request.MaxResults <- Nullable(int64 <| 50)
           request.PageToken  <- nextPageToken
        request

    type Uploads = Channel -> YouTubeService -> Async<seq<Video>>
    
    let uploads: Uploads = 
        fun channel youTubeService ->
            let playlistId = channel.ContentDetails.RelatedPlaylists.Uploads
            let playListItemConfig = configPlaylistReq playlistId
            let rec pager (acc: seq<Video>) (nextPageToken) = async {
                match nextPageToken with
                | null -> return acc
                | _ -> 
                    let playlistItemListReq = 
                        youTubeService.PlaylistItems.List(ContentStr <| Snippet)
                        |> playListItemConfig <| nextPageToken

                    let! playlistItemRep = playlistItemListReq.ExecuteAsync() |> Async.AwaitTask
                    let videos = 
                        playlistItemRep.Items
                        |> Seq.map(fun video -> { title = video.Snippet.Title; videoId = UrlPrefix + video.Snippet.ResourceId.VideoId})
                    return! pager (Seq.concat [acc; videos]) playlistItemRep.NextPageToken 
            }    
            pager [] ""

let private getFirstChannel (channelList: ChannelListResponse) = 
    Seq.tryHead channelList.Items

let uploadsOrEmpty channel youTubeService = channel |> function
    | Some c -> Playlist.uploads c youTubeService
    | None   -> async { return Seq.empty }

type UploadList = YouTubeService -> Id -> Async<seq<Video>>

let uploadList: UploadList = fun youTubeService id -> 
    async { let! channelList = Channel.list youTubeService id
            let videos = channelList |> getFirstChannel
                                     |> uploadsOrEmpty <| youTubeService
            return! videos
    }

let getVideos youtube parameters = 
    async {
        let! videos = uploadList youtube parameters
        let  out =    videos |> Seq.map(fun video -> sprintf "Title: %s\nVideoId: %s\n" video.title (UrlPrefix + video.videoId))
                             |> Seq.reduce(+)
        return out
    }   |> Async.RunSynchronously