module Nikeza.Server.Platforms

open System.IO
open Literals
open Model
open YouTube
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
    platform.ToLower() |> function
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
    | WordPress     -> KeyNotRequired
    | Medium        -> KeyNotRequired
    | RSSFeed       -> KeyNotRequired
    | Other         -> KeyNotRequired

let getThumbnail accessId platform = platform |> function
    | YouTube       -> YouTube       .getThumbnail accessId <| getKey platform
    | StackOverflow -> StackOverflow .getThumbnail accessId <| getKey platform
    | WordPress     -> WordPress     .getThumbnail accessId
    | Medium        -> Medium        .getThumbnail accessId
    | RSSFeed       -> DefaultThumbnail
    | Other         -> DefaultThumbnail

let linksFrom platformUser : Link list =

    let user =  platformUser.User
    
    platformUser.Platform |> function
    | YouTube       -> platformUser |> youtubeLinks
    | StackOverflow -> platformUser |> stackoverflowLinks
    | WordPress     -> user         |> wordpressLinks
    | Medium        -> user         |> mediumLinks
    | RSSFeed       -> user         |> rssLinks
    | Other         -> []