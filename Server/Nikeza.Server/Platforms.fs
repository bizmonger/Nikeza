module Nikeza.Server.Platforms

open System.IO
open Literals
open Model
open YouTube
open Authentication
open StackOverflow
open WordPress
open Medium
open RSSFeed

let PlatformToString = function
    | YouTube       -> "youtube"
    | WordPress     -> "wordpress"
    | StackOverflow -> "stackoverflow"
    | Medium        -> "medium"
    | RSSFeed       -> "rss feed"
    | Other         -> "other"

let platformFromString (platform:string) =
    match platform.ToLower() with
    | "youtube"       -> YouTube
    | "wordpress"     -> WordPress
    | "stackoverflow" -> StackOverflow
    | "medium"        -> Medium
    | "rss feed"      -> RSSFeed
    | "other"         -> Other
    | _               -> Other

let getKey = function
    | YouTube       -> File.ReadAllText(KeyFile_YouTube)
    | StackOverflow -> File.ReadAllText(KeyFile_StackOverflow)
    | WordPress     -> KeyNotProvided
    | Medium        -> KeyNotProvided
    | RSSFeed       -> KeyNotProvided
    | Other         -> KeyNotProvided

let getThumbnail accessId platform = platform |> function
    | YouTube       -> YouTube       .getThumbnail accessId <| getKey platform
    | StackOverflow -> StackOverflow .getThumbnail accessId <| getKey platform
    | WordPress     -> WordPress     .getThumbnail accessId
    | Medium        -> Medium        .getThumbnail accessId
    | RSSFeed       -> RSSFeed       .getThumbnail accessId
    | Other         -> DefaultThumbnail

let linkOf video profileId = {
    Id=          0
    ProfileId=   profileId
    Title=       video.Title
    Description= video.Description
    Url=         video.Url
    Topics=      video.Tags |> List.map (fun t -> { Id=0; Name=t; IsFeatured= false })
    ContentType= VideoText
    IsFeatured=  false
}

let youtubeLinks (platformUser:PlatformUser) = 
    
    let user = platformUser.User

    async { let    youtube = youTubeService platformUser.APIKey
            let!   videos =  uploadList youtube <| ChannelId user.AccessId
            return videos |> Seq.rev    
                          |> List.ofSeq 

    } |> Async.RunSynchronously
      |> List.map (fun video -> linkOf video user.ProfileId )

let linksFrom platformUser : Link list =

    let user =  platformUser.User
    
    platformUser.Platform |> function
    | YouTube       -> platformUser |> youtubeLinks
    | StackOverflow -> platformUser |> stackoverflowLinks
    | WordPress     -> []           |> wordpressLinks user 1
    | Medium        -> user         |> mediumLinks
    | RSSFeed       -> user         |> rssLinks
    | Other         -> []