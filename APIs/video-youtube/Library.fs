namespace VideoYouTube

open System
open System.IO
open FSharp.Control

open Google.Apis.Auth.OAuth2
open Google.Apis.Services
open Google.Apis.Upload
open Google.Apis.Util.Store
open Google.Apis.YouTube.v3
open Google.Apis.YouTube.v3.Data

module Data =

    type Content = 
        | Details
        | Snippet

    type Video = { title: string; videoId: string }

    let ContentStr t = 
        match t with
            | Details -> "ContentDetails"
            | Snippet -> "Snippet"

    let youTubeService apiKey =
        new YouTubeService(
            BaseClientService.Initializer(
                ApiKey = apiKey,
                ApplicationName = "NIEKEZA-video-youtube"
            )
        )

    let configurePlayListItemRequest playListId (request: PlaylistItemsResource.ListRequest) nextPageToken = 
        do 
            request.PlaylistId <- playListId
            request.MaxResults <- Nullable(int64 <| 50)
            request.PageToken <- nextPageToken
        request

    let downlUploadVidoesData (youTubeService: YouTubeService) (channel: Channel) =
        let playlistId = channel.ContentDetails.RelatedPlaylists.Uploads
        let playListItemConfig = configurePlayListItemRequest playlistId
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
                    |> Seq.map(fun video -> { title = video.Snippet.Title; videoId = video.Snippet.ResourceId.VideoId})
                return! pager (Seq.concat [acc; videos]) playlistItemRep.NextPageToken 
        }    
        pager [] ""

    type ChannelListReqParams = { id: Option<string>; userName: Option<string> }

    let getOrElse o d = match o with | Some x -> x | _ -> d

    let uploadList (youTubeService: YouTubeService) (channelReqParams: ChannelListReqParams) = 
        let downloader = downlUploadVidoesData youTubeService
        async {
            let channelListReq = youTubeService.Channels.List(ContentStr(Details))
            do 
                channelListReq.Id <- getOrElse channelReqParams.id null
                channelListReq.ForUsername <- getOrElse channelReqParams.userName null
            let! channelList = channelListReq.ExecuteAsync() |> Async.AwaitTask
            let videos = 
                channelList.Items
                |> Seq.head // We only want the uploads 
                |> downloader
            return! videos
        }
