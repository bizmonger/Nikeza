module Nikeza.Server.DataStore

open System
open System.Data.SqlClient
open Nikeza.Server.Models
open Nikeza.Server.DataRead
open Nikeza.Server.Sql

let executeNonQuery (command: SqlCommand) = command.ExecuteNonQuery() |> ignore
    
module private Store = 

    let executeQuery (command: SqlCommand) = command.ExecuteReader()

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

let findUser email :(Profile option) =
    use connection = new SqlConnection(connectionString)
    use command =    new SqlCommand(findUserByEmailSql,connection)

    command |> addWithValue "@email"  email  |> ignore
    readCommand connection command sqlReader |> Seq.tryHead
    
let private register (info:Profile) =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@FirstName"     info.FirstName
                |> addWithValue "@LastName"      info.LastName
                |> addWithValue "@Email"         info.Email
                |> addWithValue "@ImageUrl"      info.ImageUrl
                |> addWithValue "@Bio"           info.Bio
                |> addWithValue "@PasswordHash"  info.PasswordHash
                |> addWithValue "@Created"       info.Created
                |> addWithValue "@Salt"          "security_mechanism"
    
    Store.execute connectionString registerSql commandFunc

let private addLink (info:AddLinkRequest) =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@ProviderId"    info.ProviderId
                |> addWithValue "@Title"         info.Title
                |> addWithValue "@Description"   info.Description
                |> addWithValue "@Url"           info.Url
                |> addWithValue "@ContentTypeId" (info.ContentType |> contentTypeToId)
                |> addWithValue "@IsFeatured"    info.IsFeatured
                |> addWithValue "@Created"       DateTime.Now
    
    Store.execute connectionString addLinkSql commandFunc

let private removeLink (info:RemoveLinkRequest) =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@LinkId" info.LinkId
    
    Store.execute connectionString deleteLinkSql commandFunc

let private follow (info:FollowRequest) =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@SubscriberId" info.SubscriberId
                |> addWithValue "@ProviderId"   info.ProviderId
    
    Store.execute connectionString followSql commandFunc

let private unsubscribe (info:UnsubscribeRequest) =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@SubscriberId" info.SubscriberId
                |> addWithValue "@ProviderId"   info.ProviderId

    Store.execute connectionString unsubscribeSql commandFunc

let private featureLink (info:FeatureLinkRequest) =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@Id"         info.LinkId
                |> addWithValue "@IsFeatured" info.IsFeatured

    Store.execute connectionString featureLinkSql commandFunc

let private updateProfile (info:ProfileRequest) =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@Id"        info.ProfileId
                |> addWithValue "@FirstName" info.FirstName
                |> addWithValue "@LastName"  info.LastName
                |> addWithValue "@bio"       info.Bio
                |> addWithValue "@email"     info.Email

    Store.execute connectionString updateProfileSql commandFunc
    

let private addSource (info:AddSourceRequest) =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@ProfileId" info.ProfileId
                |> addWithValue "@Platform"  info.Platform
                |> addWithValue "@Username"  info.Username

    Store.execute connectionString addSourceSql commandFunc

let private removeSource (info:RemoveSourceRequest) =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@Id" info.SourceId
        
    Store.execute connectionString deleteSourceSql commandFunc

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
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@ProviderId" providerId

    let links = readInLinks |> getResults getLinksSql commandFunc
    links

let getFollowers providerId =
    let profiles = getProfiles providerId getFollowersSql "@ProviderId"
    profiles

let getSubscriptions subscriberId =
    let profiles = getProfiles subscriberId getSubscriptionsSql "@SubscriberId"
    profiles

let getProviders () =
    let commandFunc (command: SqlCommand) = command
    let providers = readInProfiles |>  getResults getProvidersSql commandFunc
    providers

let getProvider providerId =
    let profiles = getProfiles providerId getProviderSql "@ProviderId"
    profiles |> List.tryHead

let getSources providerId =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@ProfileId" providerId
        
    let sources = readInSources |> getResults getSourcesSql commandFunc
    sources

let getPlatforms () =
    let commandFunc (command: SqlCommand) = command
    let platforms = readInPlatforms |> getResults getPlatformsSql commandFunc
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