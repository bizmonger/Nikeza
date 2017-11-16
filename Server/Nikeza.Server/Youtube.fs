module Nikeza.Server.YouTube

open System
open Http
open Literals
open Asynctify
open FSharp.Control
open Google.Apis.Services
open Google.Apis.YouTube.v3
open Google.Apis.YouTube.v3.Data
open Newtonsoft.Json

[<Literal>]
let private UrlPrefix = "https://www.youtube.com/watch?v="

[<Literal>]
let private BaseAddress = "https://www.googleapis.com/youtube/v3/"

[<Literal>]
let private tagsUrl = "videos?key={0}&fields=items(snippet(title,tags))&part=snippet&id={1}"

[<Literal>]
let private thumbnailUrl = "channels?part=snippet&fields=items%2Fsnippet%2Fthumbnails%2Fdefault&id={0}&key={1}"

[<CLIMutable>]
type Default =    { url:string }

[<CLIMutable>]
type Thumbnails = { ``default``:Default }
 
[<CLIMutable>]
type Snippet = { 
    title: string
    tags: String seq
    thumbnails: Thumbnails
}

[<CLIMutable>]
type Item =       { snippet : Snippet }

[<CLIMutable>]
type Response =   { items : Item seq }

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
                    let videosWithTags = 
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
                    return! pager (Seq.concat [acc; videosWithTags]) playlistItemRep.NextPageToken 
            }    
            pager [] ""

let private getFirstChannel (channelList: ChannelListResponse) = 
    Seq.tryHead channelList.Items

let uploadsOrEmpty channel youTubeService = channel |> function
    | Some c -> Playlist.uploads c youTubeService
    | None   -> async { return Seq.empty }

let getThumbnail accessId key =

    let response = sendRequest BaseAddress thumbnailUrl accessId key
    if  response.IsSuccessStatusCode
        then let json =     response.Content.ReadAsStringAsync() |> toResult
             let settings = JsonSerializerSettings()
             settings.MissingMemberHandling <- MissingMemberHandling.Ignore

             let result = JsonConvert.DeserializeObject<Response>(json, settings)
             match result.items |> Seq.toList with
             | h::_ -> h.snippet.thumbnails.``default``.url
             | _    ->  DefaultThumbnail

        else DefaultThumbnail

let getTags apiKey videosWithTags =

      let isBlackListed tag =
          ["Hangouts On Air";"#hangoutsonair";"#hoa";"YouTube Editor"]
          |> List.contains tag 
          |> not
      let screen tags = 
          tags |> List.choose (fun tag -> if isBlackListed tag then Some tag else None )
      let getTags item =
          if item.snippet.tags |> isNull
              then []
              else item.snippet.tags 
                   |> List.ofSeq
                   |> screen
                   
      let delimitedIds = videosWithTags 
                         |> Seq.ofArray 
                         |> Seq.map (fun v -> v.Id) 
                         |> String.concat ","
      use client =   httpClient BaseAddress
      let url =      String.Format(tagsUrl, apiKey, delimitedIds)
      let response = client.GetAsync(url) |> Async.AwaitTask 
                                          |> Async.RunSynchronously
      if response.IsSuccessStatusCode
          then let json =   response.Content.ReadAsStringAsync() |> toResult
               let result = JsonConvert.DeserializeObject<Response>(json)
               let tags =   result.items |> List.ofSeq 
                                         |> List.map getTags
               tags
          else []

let applyVideoTags videoAndTags =

    let video = fst videoAndTags
    let tags  = snd videoAndTags

    { video with Tags = tags }

type UploadList = YouTubeService -> AccountId -> Async<seq<Video>>

let uploadList: UploadList = fun youTubeService id ->
    async { let!   channelList =   Channel.list youTubeService id
            let!   videos=         channelList |> getFirstChannel
                                               |> uploadsOrEmpty <| youTubeService
            let    maxRequestIdsAllowed = 50
            let    videosWithTags= videos |> Seq.chunkBySize maxRequestIdsAllowed
                                          |> Seq.collect (fun chunks -> chunks |> getTags youTubeService.ApiKey) 
                                          |> Seq.zip videos
                                          |> Seq.map applyVideoTags
            return videosWithTags
    }