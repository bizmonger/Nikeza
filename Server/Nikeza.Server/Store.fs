module Nikeza.Server.Store

open System.Data.SqlClient
open Nikeza.Server.Command
open Nikeza.Server.Read
    
module private Store = 
    let executeQuery (command: SqlCommand) = command.ExecuteReader()

    let query connectionString sql commandFunc =

        let connection = createConnection connectionString
        connection.Open()

        use command = (new SqlCommand(sql,connection)) |> commandFunc
        let reader = executeQuery command
        
        (reader,connection)

open Nikeza.Server.Model
open Nikeza.Server.Sql

let findUser email :(Profile option) =
    use connection = new SqlConnection(connectionString)
    use command =    new SqlCommand(findUserByEmailSql,connection)

    command |> addWithValue "@email"  email  |> ignore
    let result = readCommand connection command sqlReader |> Seq.tryHead
    (connection.Close() |> ignore); result
    
let getResults sql commandFunc readInData =
    let (reader, connection) = Store.query connectionString sql commandFunc
    
    let entities = 
        try     readInData [] reader
        finally reader.Dispose()
                connection.Close()
    entities

let getMemberNetwork profileId sql parameterName =
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
    let profiles = getMemberNetwork providerId getFollowersSql "@ProviderId"
    profiles

let getSubscriptions subscriberId =
    let profiles = getMemberNetwork subscriberId getSubscriptionsSql "@SubscriberId"
    profiles

let getProviders () =
    let commandFunc (command: SqlCommand) = command
    let providers = readInProviders |>  getResults getProvidersSql commandFunc
    providers

let getProvider providerId =
    let profiles = getMemberNetwork providerId getProviderSql "@ProviderId"
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

let usernameToId username =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@Email" username
        
    let profileId = readInProfileId |> getResults getUsernameToIdSql commandFunc
    profileId