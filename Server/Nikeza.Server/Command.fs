module Nikeza.Server.Command

open System
open System.IO
open System.Data.SqlClient
open Nikeza.Server.Literals
open Nikeza.Server.Model
open Nikeza.Server.Sql
open Nikeza.Server.YouTube
open Nikeza.Server.YouTube.Authentication

let dispose (connection:SqlConnection) (command:SqlCommand) =
    connection.Dispose()
    command.Dispose()

let addWithValue paramName obj (command: SqlCommand) =
    command.Parameters.AddWithValue(paramName,  obj) |> ignore
    command

let createConnection connectionString = new SqlConnection(connectionString)

module private Commands =

    let executeScalar (command: SqlCommand) = 
        let result =  (command.ExecuteScalar())
        if result |> isNull
            then None
            else Some <| string (unbox<Int32> result)

    let execute connectionString sql commandFunc = 
        use connection = createConnection connectionString
        connection.Open()

        use command = new SqlCommand(sql,connection) |> commandFunc
        executeScalar command |> function
        | Some id -> id
        | None    -> ""

    let register (info:Profile) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@FirstName"     info.FirstName
                    |> addWithValue "@LastName"      info.LastName
                    |> addWithValue "@Email"         info.Email
                    |> addWithValue "@ImageUrl"      info.ImageUrl
                    |> addWithValue "@Bio"           info.Bio
                    |> addWithValue "@PasswordHash"  info.PasswordHash
                    |> addWithValue "@Created"       info.Created
                    |> addWithValue "@Salt"          info.Salt
        
        commandFunc |> execute connectionString registerSql

    let addLinkTopics (link:Link) =
        let linkTopicIds =
            link.Topics 
            |> List.map (fun topic ->
                let linkTopicsCommandFunc (command: SqlCommand) =
                    command |> addWithValue "@LinkId"     link.Id
                            |> addWithValue "@TopicId"    topic

                linkTopicsCommandFunc |> execute connectionString addLinkTopicSql
                        )
        linkTopicIds |> String.concat ","

    let addLink (info:Link) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@ProfileId"     (Int32.Parse(info.ProfileId))
                    |> addWithValue "@Title"         info.Title
                    |> addWithValue "@Description"   info.Description
                    |> addWithValue "@Url"           info.Url
                    |> addWithValue "@ContentTypeId" (info.ContentType |> contentTypeToId)
                    |> addWithValue "@IsFeatured"    info.IsFeatured
                    |> addWithValue "@Created"       DateTime.Now
        
        let linkId = commandFunc |> execute connectionString addLinkSql

        addLinkTopics info |> ignore; linkId


    let addSource (info:DataSourceRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@ProfileId" (Int32.Parse(info.ProfileId))
                    |> addWithValue "@Platform"  info.Platform
                    |> addWithValue "@AccessId"  info.AccessId
        
        commandFunc |> execute connectionString addDataSourceSql

    let addSourceLink (source:DataSourceRequest) (link:Link) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@SourceId" source.Id
                    |> addWithValue "@LinkId"   link.Id
        
        commandFunc |> execute connectionString addSourceLinkSql

    let removeLink (info:RemoveLinkRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@LinkId" info.LinkId
        
        commandFunc |> execute connectionString deleteLinkSql

    let follow (info:FollowRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@SubscriberId" info.SubscriberId
                    |> addWithValue "@ProfileId"   info.ProfileId
        
        commandFunc |> execute connectionString followSql

    let unsubscribe (info:UnsubscribeRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@SubscriberId" info.SubscriberId
                    |> addWithValue "@ProfileId"   info.ProfileId

        commandFunc |> execute connectionString unsubscribeSql

    let observeLinks (info:ObservedLinks) =
        let ids = 
            info.LinkIds 
            |> List.map (fun linkId ->
                          let commandFunc (command: SqlCommand) = 
                              command |> addWithValue "@SubscriberId" info.SubscriberId
                                      |> addWithValue "@LinkId"       linkId
                          commandFunc |> execute connectionString observeLinkSql |> ignore
                          linkId |> string
                        )
        ids |> Seq.ofList |> String.concat ","

    let getRecent (info:RecentRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@SubscriberId" info.SubscriberId

        commandFunc |> execute connectionString getRecentSql

    let featureLink (info:FeatureLinkRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@Id"         info.LinkId
                    |> addWithValue "@IsFeatured" info.IsFeatured

        commandFunc |> execute connectionString featureLinkSql

    let updateProfile (info:ProfileRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@Id"        (Int32.Parse(info.ProfileId))
                    |> addWithValue "@FirstName" info.FirstName
                    |> addWithValue "@LastName"  info.LastName
                    |> addWithValue "@bio"       info.Bio
                    |> addWithValue "@email"     info.Email

        commandFunc |> execute connectionString updateProfileSql

    let toPlatformType = function
        | "YouTube"       -> YouTube
        | "WordPress"     -> WordPress
        | "StackOverflow" -> StackOverflow
        | _               -> Other

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
           Topics=      video.Tags |> List.map (fun t -> { Id=0; Name=t; IsFeatured=false })
           ContentType= VideoText
           IsFeatured=  false
         }

    let getLinks (source:PlatformUser) =
        source.Platform |> function
        | YouTube       ->
            let user =  source.User
            user.AccessId |> youtubeLinks source.APIKey  
                          |> Async.RunSynchronously
                          |> Seq.map (fun video -> linkOf video user.ProfileId )
                          
        | WordPress     -> Seq.empty // todo...
        | StackOverflow -> Seq.empty // todo...
        | Other         -> Seq.empty // todo...
        
    let addDataSource (info:DataSourceRequest) =

        let source = {
            ProfileId=  info.ProfileId
            Platform=   info.Platform |> toPlatformType
            APIKey=     File.ReadAllText(APIKeyFile)
            User=     { AccessId = info.AccessId; ProfileId= info.ProfileId }
        }

        let links =   source |> getLinks
        let linkIds = links  |> Seq.map addLink |> Seq.toList
        let zipped =  Seq.zip links linkIds
        let updatedLinks = 
            zipped |> Seq.map (fun linkAndId -> 
                                let link = fst linkAndId
                                let id   = snd linkAndId
                                { link with Id = Int32.Parse(id) })

        let pendingSource = { info with Links= updatedLinks }
        let sourceId =        addSource pendingSource
        let updatedSource = { pendingSource with Id = Int32.Parse(sourceId) }

        updatedLinks |> Seq.toList 
                     |> List.map (fun link -> addSourceLink updatedSource link ) 
                     |> ignore
        sourceId

    let removeDataSource (info:RemoveDataSourceRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@Id" info.Id
            
        commandFunc |> execute connectionString deleteSourceSql

open Commands
let execute = function
    | Register      info -> register         info
    | UpdateProfile info -> updateProfile    info
   
    | Follow        info -> follow           info
    | Unsubscribe   info -> unsubscribe      info
   
    | AddLink       info -> addLink          info
    | RemoveLink    info -> removeLink       info
    | FeatureLink   info -> featureLink      info
    | ObserveLinks  info -> observeLinks     info

    | AddSource     info -> addDataSource    info
    | RemoveSource  info -> removeDataSource info