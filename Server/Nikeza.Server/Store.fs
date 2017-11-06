module Nikeza.Server.Store

open System.Data.SqlClient
open CommandDetails
open Read
open Converters

module private Store = 
    let executeQuery (command: SqlCommand) = command.ExecuteReader()

    let query connectionString sql commandFunc =

        let connection = createConnection connectionString
        connection.Open()

        use command = (new SqlCommand(sql,connection)) |> commandFunc
        let reader = executeQuery command
        
        (reader,connection)

open Model
open Sql
    
let getResults sql commandFunc readInData =
    let (reader, connection) = Store.query connectionString sql commandFunc
    
    let entities = 
        try     readInData [] reader
        finally reader.Dispose()
                connection.Close()
    entities

let linksFrom platform profileId =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@ProfileId" profileId
                |> addWithValue "@Platform"  platform

    let links = readInLinks |> getResults getSourceLinksSql commandFunc
    links

let getSource sourceId =
    let sourceCommandFunc (command: SqlCommand) = 
        command |> addWithValue "@SourceId" sourceId
        
    let sources = readInSources |> getResults getSourceSql sourceCommandFunc
    sources |> List.tryHead
            |> function
            | Some source -> let links = linksFrom source.Platform source.ProfileId
                             Some { source with Links= links }
            | None -> None

let getSources profileId =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@ProfileId" profileId
        
    let sources = readInSources |> getResults getSourcesSql commandFunc
                                |> List.map (fun s -> s.Id |> getSource)
    sources

let loginProfile email =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@Email" email
        
    readInProfiles |> getResults findUserByEmailSql commandFunc
                   |> List.tryHead
                   |> function
                      | Some p -> let sources = getSources p.ProfileId |> List.choose id
                                  Some { p with Sources = sources }
                      | None   -> None

let getProfiles profileId sql parameterName =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue parameterName profileId
        
    let profiles = readInProfiles |> getResults sql commandFunc
    profiles

let getTopics name sql parameterName =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue parameterName name
        
    let topics = readInTopics |> getResults sql commandFunc
    topics
    
let getLinks (profileId:string) =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@ProfileId" profileId

    let links = readInLinks |> getResults getLinksSql commandFunc
    links

let getPortfolio profileId = 

    let links = getLinks profileId

    { Answers =  links |> List.filter (fun l -> l.ContentType |> contentTypeFromString = Answer)
      Articles = links |> List.filter (fun l -> l.ContentType |> contentTypeFromString = Article)
      Videos =   links |> List.filter (fun l -> l.ContentType |> contentTypeFromString = Video)
      Podcasts = links |> List.filter (fun l -> l.ContentType |> contentTypeFromString = Podcast)
    }

let getRecent subscriberId =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@SubscriberId" subscriberId

    let links = readInLinks |> getResults getRecentSql commandFunc
    links

let loginProvider email =
    email |> loginProfile |> function
    | Some profile ->
        Some { Profile=        profile |> toProfileRequest
               Topics=         [] // Todo: Derive distinct topics from links
               Portfolio=      getPortfolio profile.ProfileId
               RecentLinks=    getRecent    profile.ProfileId
               Subscriptions=  []
               Followers=      []
            }
    | None -> None

let getFollowers profileId =
    let profiles = getProfiles profileId getFollowersSql "@ProfileId"
    profiles

let getSubscriptions subscriberId =
    let profiles = getProfiles subscriberId getSubscriptionsSql "@SubscriberId"
    profiles

let getProviders () =
    let commandFunc (command: SqlCommand) = command
    let providers = readInProviders |> getResults getProfilesSql commandFunc
    providers

let getProfile profileId =
    let profiles = getProfiles profileId getProfileSql "@ProfileId"
    profiles |> List.tryHead

let getTopic name =
    let topics = getTopics name getTopicSql "@Name"
    topics |> List.tryHead

let getAllProfiles () =
    let commandFunc (command: SqlCommand) = command
    let profiles = readInProfiles |> getResults getProfilesSql commandFunc
    profiles

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