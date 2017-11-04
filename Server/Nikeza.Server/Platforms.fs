module Nikeza.Server.Platforms

open System.IO
open Literals
open Model
open YouTube
open Authentication
open StackOverflow
open Wordpress
open Medium

let PlatformToString = function
    | YouTube       -> "youtube"
    | WordPress     -> "wordpress"
    | StackOverflow -> "stackoverflow"
    | Medium        -> "medium"
    | Other         -> "other"

let platformFromString (platform:string) =
    match platform.ToLower() with
    | "youtube"       -> YouTube
    | "wordpress"     -> WordPress
    | "stackoverflow" -> StackOverflow
    | "medium"        -> Medium
    | "other"         -> Other
    | _               -> Other

let getKey = function
    | YouTube       -> File.ReadAllText(KeyFile_YouTube)
    | StackOverflow -> File.ReadAllText(KeyFile_StackOverflow)
    | WordPress     -> "no key provided"
    | Medium        -> "no key provided"
    | Other         -> "no key provided"

let getThumbnail accessId platform = platform |> function
    | YouTube       -> YouTube       .getThumbnail accessId <| getKey platform
    | StackOverflow -> StackOverflow .getThumbnail accessId <| getKey platform
    | WordPress     -> ThumbnailUrl
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

let linksFrom (platformUser:PlatformUser) =

    let user =  platformUser.User
    
    platformUser.Platform |> function
    | YouTube       -> user.AccessId |> youtubeLinks platformUser.APIKey  
                                     |> Async.RunSynchronously
                                     |> Seq.map (fun video -> linkOf video user.ProfileId )
                       
    | StackOverflow -> platformUser |> stackoverflowLinks
    | WordPress     -> []           |> wordpressLinks user 1
    | Medium        -> user         |> mediumLinks
    | Other         -> Seq.empty