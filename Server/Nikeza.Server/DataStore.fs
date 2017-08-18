module Nikeza.Server.DataStore

open System
open System.Data.SqlClient
open Nikeza.Server.Models
open Nikeza.Server.Sql

let dispose (connection:SqlConnection) (command:SqlCommand) =
    connection.Dispose()
    command.Dispose()
    
module private Store = 

    let private executeNonQuery (command: SqlCommand) = command.ExecuteNonQuery() |> ignore
    
    let private executeQuery (command: SqlCommand) = command.ExecuteReader()

    let createConnection connectionString =
        let connection = new SqlConnection(connectionString)
        connection

    let execute connectionString sql commandFunc = 
        use connection = createConnection connectionString
        connection.Open()

        use command = (new SqlCommand(sql,connection)) |> commandFunc
        executeNonQuery command

    let query connectionString sql commandFunc =

        let connection = createConnection connectionString
        connection.Open()

        use command = (new SqlCommand(sql,connection)) |> commandFunc
        let reader = executeQuery command
        
        (reader,connection)

type ContentType = 
    | Article
    | Video
    | Answer
    | Podcast
    | Unknown

type RawContentType = string

let contentTypeFromString = function
    | "article" -> Article
    | "video"   -> Video
    | "answer"  -> Answer
    | "podcast" -> Podcast
    | _         -> Unknown

let contentTypeToId = function
    | "article" ->  0
    | "video"   ->  1
    | "answer"  ->  2
    | "podcast" ->  3
    | _         -> -1

let contentTypeToString = function
    | Article -> "article"
    | Video   -> "video"  
    | Answer  -> "answer" 
    | Podcast -> "podcast"
    | Unknown -> "unknown"    

let contentTypeIdToString = function
    | 0 -> "article"
    | 1 -> "video"  
    | 2 -> "answer" 
    | 3 -> "podcast"
    | _ -> "unknown"        

let addWithValue paramName obj (command: SqlCommand) =
    command.Parameters.AddWithValue(paramName,  obj) |> ignore
    command

let readCommand (connection: SqlConnection) (command: SqlCommand) readerFunc =
    if connection.State = System.Data.ConnectionState.Closed
    then connection.Open()

    let reader = command.ExecuteReader()
    let data = seq { while reader.Read() do yield readerFunc(reader) }
    connection.Close() |> ignore
    data

let private executeNonQuery (command: SqlCommand) = command.ExecuteNonQuery() |> ignore

let createCommand sql =
    let connection = new SqlConnection(connectionString)
    let command =    new SqlCommand(sql,connection)
    (connection, command)

let findUser email :(Profile option) =
    use connection = new SqlConnection(connectionString)
    use command = new SqlCommand(findUserByEmailSql,connection)

    command |> addWithValue "@email"  email
            |> ignore

    let sqlReader (reader: SqlDataReader) = { 
        ProfileId =    reader.["Id"].ToString() |> int
        FirstName =    reader.["FirstName"].ToString()
        LastName =     reader.["LastName"].ToString()
        Email =        reader.["Email"].ToString()
        ImageUrl =     reader.["ImageUrl"].ToString()
        Bio =          reader.["Bio"].ToString()
        PasswordHash = reader.["PasswordHash"].ToString()
        Salt =         reader.["Salt"].ToString()
        Created =      DateTime.Parse(reader.["Created"].ToString()) 
    }

    readCommand connection command sqlReader |> Seq.tryHead
    
let private register (info:Profile) =

    let sql = registerSql
    let (connection,command) = createCommand(sql)
    try connection.Open()
        command |> addWithValue "@FirstName"     info.FirstName
                |> addWithValue "@LastName"      info.LastName
                |> addWithValue "@Email"         info.Email
                |> addWithValue "@ImageUrl"      info.ImageUrl
                |> addWithValue "@Bio"           info.Bio
                |> addWithValue "@PasswordHash"  info.PasswordHash
                |> addWithValue "@Created"       info.Created
                |> addWithValue "@Salt"          "security_mechanism"
                |> executeNonQuery

    finally dispose connection command

let private addLink (info:AddLinkRequest) =
    let sql = addLinkSql
    let (connection,command) = createCommand(sql)

    try connection.Open()
        command |> addWithValue "@ProviderId"    info.ProviderId
                |> addWithValue "@Title"         info.Title
                |> addWithValue "@Description"   info.Description
                |> addWithValue "@Url"           info.Url
                |> addWithValue "@ContentTypeId" (info.ContentType |> contentTypeToId)
                |> addWithValue "@IsFeatured"    info.IsFeatured
                |> addWithValue "@Created"       DateTime.Now
                |> executeNonQuery

    finally dispose connection command

let private removeLink (info:RemoveLinkRequest) =
    let sql = deleteLinkSql
    let (connection,command) = createCommand(sql)

    try connection.Open()
        command |> addWithValue "@LinkId" info.LinkId
                |> executeNonQuery

    finally dispose connection command

let private follow (info:FollowRequest) =
    let sql = followSql
    let (connection,command) = createCommand(sql)

    try connection.Open()
        command |> addWithValue "@SubscriberId"  info.SubscriberId
                |> addWithValue "@ProviderId"    info.ProviderId
                |> executeNonQuery

    finally dispose connection command

let private unsubscribe (info:UnsubscribeRequest) =
    let sql = unsubscribeSql
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@SubscriberId"  info.SubscriberId
                |> addWithValue "@ProviderId"    info.ProviderId

    Store.execute connectionString sql commandFunc

let private featureLink (info:FeatureLinkRequest) =
    let sql = featureLinkSql
    let (connection,command) = createCommand(sql)

    try connection.Open()
        command |> addWithValue "@Id"          info.LinkId
                |> addWithValue "@IsFeatured"  info.IsFeatured
                |> executeNonQuery

    finally dispose connection command

let private updateProfile (info:ProfileRequest) =
    let sql = updateProfileSql
    let (connection,command) = createCommand(sql)

    try connection.Open()
        command |> addWithValue "@Id"        info.ProfileId
                |> addWithValue "@FirstName" info.FirstName
                |> addWithValue "@LastName"  info.LastName
                |> addWithValue "@bio"       info.Bio
                |> addWithValue "@email"     info.Email
                |> executeNonQuery

    finally dispose connection command

let private addSource (info:AddSourceRequest) =
    let sql = addSourceSql
    let (connection,command) = createCommand(sql)

    try connection.Open()
        command |> addWithValue "@ProfileId" info.ProfileId
                |> addWithValue "@Platform"  info.Platform
                |> addWithValue "@Username"  info.Username
                |> executeNonQuery

    finally dispose connection command

let private removeSource (info:RemoveSourceRequest) =
    let sql = deleteSourceSql
    let (connection,command) = createCommand(sql)

    try connection.Open()
        command |> addWithValue "@Id" info.SourceId
                |> executeNonQuery

    finally dispose connection command

let rec readInLinks links (reader:SqlDataReader) =

    if reader.Read() then
    
        let link = { 
              Id =            reader.GetInt32  (0)
              ProviderId =    reader.GetInt32  (1)
              Title =         reader.GetString (2)
              Description =   reader.GetString (3)
              Url =           reader.GetString (4)
              ContentType =   reader.GetInt32  (5) |> contentTypeIdToString
              IsFeatured =    reader.GetBoolean(6)
        }
        readInLinks (link::links) reader

    else links

let rec readInProfiles profiles (reader:SqlDataReader) =

    if reader.Read() then
    
        let profile : ProfileRequest = {
            ProfileId=  reader.GetInt32 (0)
            FirstName=  reader.GetString(1)
            LastName=   reader.GetString(2)
            Email=      reader.GetString(3)
            ImageUrl=   reader.GetString(4)
            Bio=        reader.GetString(5)
        }

        readInProfiles (profile::profiles) reader
    else profiles

let rec readInSources sources (reader:SqlDataReader) =

    if reader.Read()
    then let source : AddSourceRequest = {
            ProfileId= reader.GetInt32  (0)
            Platform=   reader.GetString(1)
            Username=   reader.GetString(2)
         }
         readInSources (source::sources) reader
    else sources

let rec readInPlatforms platforms (reader:SqlDataReader) =

    if   reader.Read() 
    then readInPlatforms (reader.GetString (0)::platforms) reader
    else platforms

let getResults sql commandFunc readInData =
    let (reader, connection) = Store.query connectionString sql commandFunc
    
    let entities = 
        try     readInData [] reader
        finally reader.Dispose()
                connection.Close()
    entities

let getProfiles profileId sql parameterName =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue parameterName profileId
        
    let profiles = readInProfiles |> getResults sql commandFunc
    profiles
    
let getLinks providerId =
    let sql = getLinksSql
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@ProviderId" providerId

    let links = readInLinks |> getResults sql commandFunc
    links

let getFollowers providerId =
    let profiles = getProfiles providerId getFollowersSql "@ProviderId"
    profiles

let getSubscriptions subscriberId =
    let profiles = getProfiles subscriberId getSubscriptionsSql "@SubscriberId"
    profiles

let getProviders () =
    let sql = getProvidersSql
    let commandFunc (command: SqlCommand) = command
    let providers = readInProfiles |>  getResults sql commandFunc
    providers

let getProvider providerId =
    let profiles = getProfiles providerId getProviderSql "@ProviderId"
    profiles |> List.tryHead

let getSources providerId =
    let sql = getSourcesSql
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@ProfileId" providerId
        
    let sources = readInSources |> getResults sql commandFunc
    sources

let getPlatforms () =
    let sql = getPlatformsSql
    let commandFunc (command: SqlCommand) = command
    let platforms = readInPlatforms |> getResults sql commandFunc
    platforms

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