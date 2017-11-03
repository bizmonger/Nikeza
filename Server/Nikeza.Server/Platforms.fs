module Nikeza.Server.Platforms

open System.IO
open Nikeza.Server.Literals
open Nikeza.Server.Model
open Nikeza.Server.YouTube
open Nikeza.Server.YouTube.Authentication
open Nikeza.Server.StackOverflow
open Nikeza.Server.Wordpress
open Nikeza.Server.Medium

let PlatformToString = function
    | YouTube       -> "youtube"
    | WordPress     -> "wordpress"
    | StackOverflow -> "stackoverflow"
    | Medium        -> "Medium"
    | Other         -> "other"

let PlatformFromString (platform:string) =
    match platform.ToLower() with
    | "youtube"       -> YouTube
    | "wordpress"     -> WordPress
    | "stackoverflow" -> StackOverflow
    | "medium"        -> Medium
    | "other"         -> Other
    | _               -> Other

let getThumbnail platform accessId = platform |> function
    | YouTube       -> YouTube       .getThumbnail accessId (File.ReadAllText(KeyFile_YouTube));
    | StackOverflow -> StackOverflow .getThumbnail accessId (File.ReadAllText(KeyFile_StackOverflow));
    | Wordpress     -> ThumbnailUrl
    | Medium        -> ThumbnailUrl
    | Other         -> ThumbnailUrl

let youtubeLinks apiKey channelId = 
    async { let    youtube = youTubeService apiKey
            let!   videos =  uploadList youtube <| ChannelId channelId
            return videos
    }

let linkOf video profileId =
     { Id=          0
       ProfileId=   profileId
       Title=       video.Title
       Description= video.Description
       Url=         video.Url
       Topics=      video.Tags |> List.map (fun t -> { Id=0; Name=t })
       ContentType= VideoText
       IsFeatured=  false
     }

let getLinks (source:PlatformUser) =

    let user =  source.User
    
    source.Platform |> function
    | YouTube       -> user.AccessId |> youtubeLinks source.APIKey  
                       |> Async.RunSynchronously
                       |> Seq.map (fun video -> linkOf video user.ProfileId )
                       
    | StackOverflow -> source |> stackoverflowLinks
    | WordPress     -> []     |> wordpressLinks user 1
    | Medium        -> user   |> mediumLinks
    | Other         -> Seq.empty