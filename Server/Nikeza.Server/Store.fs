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

let attachTopics (link:Link) =
    let linkTopicsCommandFunc (command: SqlCommand) = 
        command |> addWithValue "@LinkId" link.Id
                             
    let topics = readInLinkTopics |> getResults getLinkTopicsSql linkTopicsCommandFunc
    { link with Topics= topics }

let linksFrom platform profileId =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@ProfileId" profileId
                |> addWithValue "@Platform"  platform

    readInLinks |> getResults getSourceLinksSql commandFunc
                |> List.map attachTopics

let linksFromSource sourceId sql =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@SourceId" sourceId

    readInLinks |> getResults sql commandFunc
                |> List.map attachTopics

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

let getProfileByEmail email =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@Email" email
        
    readInProfiles |> getResults findUserByEmailSql commandFunc
                   |> List.tryHead
                   |> function
                      | Some p -> Some p
                      | None   -> None

let loginProfile email =
    email |> getProfileByEmail
          |> function
             | Some p -> let sources = getSources p.Id |> List.choose id
                         Some { p with Sources = sources }
             | None   -> None

let getProfiles profileId sql parameterName =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue parameterName profileId
        
    let profiles = readInProfiles |> getResults sql commandFunc
    profiles

let getTopics topicName sql parameterName =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue parameterName topicName
        
    let topics = readInTopics |> getResults sql commandFunc
    topics

let getProviderTopics (profileId:string) =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@ProfileId" profileId

    let topics = readInTopics |> getResults getProviderTopicsSql commandFunc
    topics
    
let getLinks (profileId:string) =
    let linksCommandFunc (command: SqlCommand) = 
        command |> addWithValue "@ProfileId" profileId

    readInLinks |> getResults getLinksSql linksCommandFunc 
                |> List.map attachTopics    

let toPortfolio links = 
    { Answers =  links |> List.filter (fun l -> l.ContentType |> contentTypeFromString = Answer)
      Articles = links |> List.filter (fun l -> l.ContentType |> contentTypeFromString = Article)
      Videos =   links |> List.filter (fun l -> l.ContentType |> contentTypeFromString = ContentType.Video)
      Podcasts = links |> List.filter (fun l -> l.ContentType |> contentTypeFromString = Podcast)
    }

let getRecent subscriberId =
    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@SubscriberId" subscriberId

    let links = readInLinks |> getResults getRecentSql commandFunc
    links

let getFeaturedTopics (profileId:string) =

    let commandFunc (command: SqlCommand) =
        command |> addWithValue "@ProfileId" profileId

    let featuredTopics = readInFeaturedTopics 
                          |> getResults getFeaturedTopicsSql commandFunc
                          |> List.map (fun t -> { Id= t.TopicId
                                                  Name= t.Name
                                                  IsFeatured= true })
    featuredTopics

let recentLinks links =

    let latest = links |> List.sortByDescending (fun l -> l.Timestamp)
    latest |> List.length
           |> function
              | 0 -> []
              | 1 -> latest |> List.take 1
              | 2 -> latest |> List.take 2
              | _ -> latest |> List.take 3

let rec getProvidersHelper sql parameterName profileId =

    let commandFunc (command: SqlCommand) = 
        if parameterName <> "" && profileId <> "" 
           then command |> addWithValue parameterName profileId
           else command
     
    let initialProviders = readInProviders  |> getResults sql commandFunc
    let providers =        initialProviders 
                            |> List.map (fun p -> p.Profile.Id |> getFeaturedTopics)
                            |> List.zip initialProviders
                            |> List.map (fun (p,t) -> 

                                { p with Topics= t
                                         RecentLinks= p.Profile.Id 
                                                        |> getLinks 
                                                        |> recentLinks 
                                } 
                                         
    providers

and getSubscriptions profileId : ProviderRequest list =
    let providers = getProvidersHelper getSubscriptionsSql "SubscriberId" profileId
    providers

and getFollowers profileId : ProviderRequest list =
    let providers = getProvidersHelper getFollowersSql "@ProfileId" profileId
    providers

and getProviders () =
    let providers = getProvidersHelper getProfilesSql "" ""
    providers

and hydrate (profile:ProfileRequest) =

    let links =         profile.Id |> getLinks
    let subscriptions = profile.Id |> getSubscriptions |> List.map(fun s -> s.Profile.Id)
    let followers =     profile.Id |> getFollowers     |> List.map(fun s -> s.Profile.Id)

    { Profile=       profile
      Topics=        profile.Id |> getFeaturedTopics
      Portfolio=     links      |> toPortfolio
      RecentLinks=   links      |> recentLinks
      Subscriptions= subscriptions 
      Followers=     followers     
    }

let getProvider profileId =

    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@ProfileId" profileId
        
    readInProviders 
     |> getResults getProfileSql commandFunc
     |> function 
        | p::_ -> 
            Some { p with Topics=      profileId |> getFeaturedTopics
                          Followers=   profileId |> getFollowers |> List.map (fun f -> f.Profile.Id)
                          Portfolio=   profileId |> getLinks |> toPortfolio
                          RecentLinks= profileId |> getRecent
                 }
        | _ -> None

let login email =
    email |> loginProfile 
          |> function
             | Some p -> match getProvider p.Id with
                         | Some provider -> Some { provider with Profile = p |> toProfileRequest }
                         | None          -> None
             | None -> None
let getProfile profileId =
    let profiles = getProfiles profileId getProfileSql "@ProfileId"
    profiles |> List.tryHead

let getTopic (topicName:string) =
    let topics = getTopics topicName getTopicSql "@Name"
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