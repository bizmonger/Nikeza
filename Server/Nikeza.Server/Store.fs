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

let getProfiles profileId sql parameterName =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue parameterName profileId
        
    let profiles = readInProfiles |> getResults sql commandFunc
    profiles
    
let getLinks profileId =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@ProfileId" profileId

    let links = readInLinks |> getResults getLinksSql commandFunc
    links

let getPlatformLinks profileId platform =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@ProfileId" profileId
                |> addWithValue "@Platform"  platform

    let links = readInLinks |> getResults getSourceLinksSql commandFunc
    links

let getFollowers profileId =
    let profiles = getProfiles profileId getFollowersSql "@ProfileId"
    profiles

let getSubscriptions subscriberId =
    let profiles = getProfiles subscriberId getSubscriptionsSql "@SubscriberId"
    profiles

let getProviders () =
    let commandFunc (command: SqlCommand) = command
    let providers = readInProviders |> getResults getProvidersSql commandFunc
    providers

let getProfile profileId =
    let profiles = getProfiles profileId getProfileSql "@ProfileId"
    profiles |> List.tryHead

let getAllProfiles () =
    let commandFunc (command: SqlCommand) = command
    let profiles = readInProfiles |> getResults getProfilesSql commandFunc
    profiles

let getSources profileId =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@ProfileId" profileId
        
    let sources = readInSources |> getResults getSourcesSql commandFunc
    sources

let getSource sourceId =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@SourceId" sourceId
        
    let sources = readInSources |> getResults getSourceSql commandFunc
    sources |> List.tryHead

let getPlatforms () =
    let commandFunc (command: SqlCommand) = command
    let platforms = readInPlatforms |> getResults getPlatformsSql commandFunc
    platforms

let usernameToId username =
    use connection = new SqlConnection(connectionString)
    use command =    new SqlCommand(findUserByEmailSql,connection)
    command |> addWithValue "@email"  username  |> ignore

    let result = readCommand connection command sqlReader |> Seq.tryHead
    (connection.Close() |> ignore); result