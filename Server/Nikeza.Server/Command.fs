module Nikeza.Server.Command

open System
open System.Data.SqlClient
open Nikeza.Server.Model
open Nikeza.Server.Sql

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

    let addLink (info:AddLinkRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@ProfileId"     info.ProfileId
                    |> addWithValue "@Title"         info.Title
                    |> addWithValue "@Description"   info.Description
                    |> addWithValue "@Url"           info.Url
                    |> addWithValue "@ContentTypeId" (info.ContentType |> contentTypeToId)
                    |> addWithValue "@IsFeatured"    info.IsFeatured
                    |> addWithValue "@Created"       DateTime.Now
        
        commandFunc |> execute connectionString addLinkSql

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

    let toLinks (platformUser:PlatformUser) =
        platformUser.Platform |> function
        | YouTube       -> [] // todo...
        | WordPress     -> [] // todo...
        | StackOverflow -> [] // todo...
        | Other         -> [] // todo...
        
    let addSource (info:SourceRequest) =

        { Platform= info.Platform |> toPlatformType; User= info.Username }
        |> toLinks
        |> List.map addLink 
        |> ignore

        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@ProfileId" info.ProfileId
                    |> addWithValue "@Platform"  info.Platform
                    |> addWithValue "@Username"  info.Username

        execute connectionString addSourceSql commandFunc

    let removeSource (info:RemoveSourceRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@Id" info.Id
            
        commandFunc |> execute connectionString deleteSourceSql

open Commands
let execute = function
    | Register      info -> register      info
    | UpdateProfile info -> updateProfile info

    | Follow        info -> follow        info
    | Unsubscribe   info -> unsubscribe   info

    | AddLink       info -> addLink       info
    | RemoveLink    info -> removeLink    info
    | FeatureLink   info -> featureLink   info

    | AddSource     info -> addSource     info
    | RemoveSource  info -> removeSource  info