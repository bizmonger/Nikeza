module Nikeza.Server.Command

open System
open System.Data.SqlClient
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
                    |> addWithValue "@Salt"          "security_mechanism"
        
        commandFunc |> execute connectionString registerSql

    let addLink (info:Link) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@ProfileId"     info.ProfileId
                    |> addWithValue "@Title"         info.Title
                    |> addWithValue "@Description"   info.Description
                    |> addWithValue "@Url"           info.Url
                    |> addWithValue "@ContentTypeId" (info.ContentType |> contentTypeToId)
                    |> addWithValue "@IsFeatured"    info.IsFeatured
                    |> addWithValue "@Created"       DateTime.Now
        
        commandFunc |> execute connectionString addLinkSql

    let addSource (info:PlatformUser) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@ProfileId"     info.ProfileId
                    |> addWithValue "@Platform"      info.Platform
                    |> addWithValue "@AccessId"      info.User.AccessId
                    |> addWithValue "@APIKey"        info.APIKey
        
        commandFunc |> execute connectionString addDataSourceSql

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

    let featureLink (info:FeatureLinkRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@Id"         info.LinkId
                    |> addWithValue "@IsFeatured" info.IsFeatured

        commandFunc |> execute connectionString featureLinkSql

    let updateProfile (info:ProfileRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@Id"        info.ProfileId
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

    let videoToLink video profileId =
         { Id=          0
           ProfileId=   profileId
           Title=       video.Title
           Description= video.Description
           Url=         video.Url
           Topics=      video.Tags |> List.map (fun t -> { Id=0; Name=t; IsFeatured=false })
           ContentType= VideoText
           IsFeatured=  false
         }

    let getLinks (platformUser:PlatformUser) =
        platformUser.Platform |> function
        | YouTube       ->
            let  user = platformUser.User
            let links = user.AccessId |> youtubeLinks platformUser.APIKey  
                                      |> Async.RunSynchronously
                                      |> Seq.map (fun v -> videoToLink v user.ProfileId )
            links

        | WordPress     -> Seq.empty // todo...
        | StackOverflow -> Seq.empty // todo...
        | Other         -> Seq.empty // todo...
        
    let addDataSource (info:DataSourceRequest) =

        let source = {
            ProfileId=  info.ProfileId
            Platform=   info.Platform |> toPlatformType
            APIKey=     info.APIKey
            User=     { AccessId = info.AccessId; ProfileId= info.ProfileId }
        }

        source |> getLinks
               |> Seq.map addLink
               |> ignore

        addSource source

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

    | AddSource     info -> addDataSource    info
    | RemoveSource  info -> removeDataSource info