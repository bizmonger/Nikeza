module Nikeza.Server.YouTube

open System
open System.Net.Http
open System.Net.Http.Headers
open System.Net.Http.Formatting
open FSharp.Control
open Google.Apis.Services
open Google.Apis.YouTube.v3
open Google.Apis.YouTube.v3.Data
open Newtonsoft.Json

[<Literal>]
let UrlPrefix = "https://www.youtube.com/watch?v="

[<Literal>]
let BaseAddress = "https://www.googleapis.com/youtube/v3/"

[<Literal>]
let requestTagsUrl = "https://www.googleapis.com/youtube/v3/videos?key={0}&fields=items(snippet(title,tags))&part=snippet&id={1}"

[<CLIMutable>]
type Snippet =    { title: string; tags: Collections.Generic.List<String> }

[<CLIMutable>]
type Item =       { snippet : Snippet }

[<CLIMutable>]
type Response =   { items : Collections.Generic.List<Item> }

type Video = {
    Id:          string
    Title:       string
    Url:         string
    Description: string
    PostDate:    string
    Tags:        string list
}

type Content =  Details | Snippet

type AccountId = 
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

    type private RequestChannelById = YouTubeService -> AccountId -> Async<ChannelListResponse>
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
                        |> Seq.map(fun video -> 
                            let snippet = video.Snippet
                            
                            { Id=          snippet.ResourceId.VideoId
                              Title=       snippet.Title
                              Url=         UrlPrefix + snippet.ResourceId.VideoId
                              Description= snippet.Description
                              PostDate=    (snippet.PublishedAt.Value.ToString("d"))
                              Tags = [] 
                            })
                    return! pager (Seq.concat [acc; videos]) playlistItemRep.NextPageToken 
            }    
            pager [] ""

let private getFirstChannel (channelList: ChannelListResponse) = 
    Seq.tryHead channelList.Items

let uploadsOrEmpty channel youTubeService = channel |> function
    | Some c -> Playlist.uploads c youTubeService
    | None   -> async { return Seq.empty }

type UploadList = YouTubeService -> AccountId -> Async<seq<Video>>

let uploadList: UploadList = fun youTubeService id -> 
    async { let! channelList = Channel.list youTubeService id
            let videos = channelList |> getFirstChannel
                                     |> uploadsOrEmpty <| youTubeService
            return! videos
    }

let httpClient =
    let client = new HttpClient()
    client.BaseAddress <- Uri(BaseAddress)
    client.DefaultRequestHeaders.Accept.Clear()
    client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue("application/json"))
    client

let getVideos apiKey youtube parameters = 

    let tagsfrom (videos:Video seq) =
        let  delimitedIds = videos |> Seq.map (fun v -> v.Id) |> String.concat ","
        let  url = String.Format(requestTagsUrl, apiKey, delimitedIds)
        let response = httpClient.GetAsync(url) |> Async.AwaitTask 
                                                |> Async.RunSynchronously
        if response.IsSuccessStatusCode
            then let json = response.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
                 let result = JsonConvert.DeserializeObject<Response>(json)
                 let tags = result.items |> List.ofSeq 
                                         |> List.map (fun item -> item.snippet.tags)
                 tags
            else []

    async {
        let! videos = uploadList youtube parameters
        let  tags = tagsfrom videos
        let  out =  videos 
                    |> Seq.map(fun video -> 
                        sprintf "Id: %s\nTitle: %s\nUrl: %s\nDescription: %s\nPostdate: %s"
                                video.Id
                                video.Title 
                                (sprintf "%s%s" UrlPrefix video.Url)
                                video.Description
                                video.PostDate
                               )
                    |> Seq.reduce(+)
        return out
        }   |> Async.RunSynchronously