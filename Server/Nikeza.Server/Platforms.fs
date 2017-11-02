module Nikeza.Server.Platforms

open System.IO
open Nikeza.Server.Literals
open Nikeza.Server.Model
open Nikeza.Server.YouTube
open Nikeza.Server.YouTube.Authentication
open Nikeza.Server.StackOverflow
open Nikeza.Server.Wordpress

let toPlatformType = function
    | "YouTube"       -> YouTube
    | "WordPress"     -> WordPress
    | "StackOverflow" -> StackOverflow
    | _               -> Other

let getThumbnail platform accessId = platform |> function
    | YouTube       -> YouTube       .getThumbnail accessId (File.ReadAllText(KeyFile_YouTube));
    | StackOverflow -> StackOverflow .getThumbnail accessId (File.ReadAllText(KeyFile_StackOverflow));
    | Wordpress     -> ThumbnailUrl
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
    | Other         -> Seq.empty