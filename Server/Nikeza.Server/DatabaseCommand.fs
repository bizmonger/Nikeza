module Nikeza.Server.DatabaseCommand

open System
open System.Data.SqlClient
open Model
open Store
open Sql
open Platforms

let dispose (connection:SqlConnection) (command:SqlCommand) =
    connection.Dispose()
    command.Dispose()

let addWithValue paramName obj (command: SqlCommand) =
    command.Parameters.AddWithValue(paramName,  obj) |> ignore
    command

let createConnection connectionString = new SqlConnection(connectionString)

module internal Commands =

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

    let addLinkTopic (linkTopic:LinkTopic) =

        let linkTopicsCommandFunc (command: SqlCommand) =
            command |> addWithValue "@LinkId"  linkTopic.Link.Id
                    |> addWithValue "@TopicId" linkTopic.Topic.Id

        linkTopicsCommandFunc |> execute connectionString addLinkTopicSql

    let addTopic (info:TopicRequest) =

        let topicName = info.Name.ToLower().Replace(" ", "-")

        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@Name" topicName

        topicName 
         |> getTopic 
         |> function
            |None   -> commandFunc |> execute connectionString addTopicSql
            |Some t -> t.Id |> string

    let featureTopic (info:ProviderTopicRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@ProfileId"  info.ProfileId
                    |> addWithValue "@TopicId"    info.TopicId

        commandFunc |> execute connectionString featureTopicSql

    let unfeatureTopic (info:ProviderTopicRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@ProfileId"  info.ProfileId
                    |> addWithValue "@TopicId"    info.TopicId

        commandFunc |> execute connectionString unfeatureTopicSql

    let clearFeaturedTopics profileId =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@ProfileId"  profileId

        commandFunc |> execute connectionString clearFeaturedTopicsSql

    let addLink (info:Link) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@ProfileId"     (Int32.Parse(info.ProfileId))
                    |> addWithValue "@Title"         info.Title
                    |> addWithValue "@Description"   info.Description
                    |> addWithValue "@Url"           info.Url
                    |> addWithValue "@ContentTypeId" (info.ContentType |> contentTypeToId)
                    |> addWithValue "@IsFeatured"    info.IsFeatured
                    |> addWithValue "@Created"       info.Timestamp
                    
        let linkId = commandFunc |> execute connectionString addLinkSql

        let addTopic (providerTopic:ProviderTopic)  =
            let temp =      { Link= info; Topic= { Id= -1; Name= providerTopic.Name.ToLower()} }   
            let topicId =   addTopic { Name= providerTopic.Name }
            let link=       { temp.Link  with Id= Int32.Parse(linkId) }
            let topic=      { temp.Topic with Id= Int32.Parse(topicId) }
            let linkTopic = { Link= link; Topic= topic }

            addLinkTopic linkTopic |> ignore

            if providerTopic.IsFeatured 
               then featureTopic { TopicId=Int32.Parse(topicId)
                                   ProfileId=link.ProfileId 
                                   Name= topic.Name
                                   IsFeatured= true
                                 } |> ignore
               else ()

        info.Topics |> List.iter addTopic
        linkId

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

    let getSyncHistory sourceId =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@SourceId" sourceId
        
        commandFunc |> execute connectionString getSyncHistorySql

    let updateSyncHistory sourceId =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@SourceId" sourceId
        
        commandFunc |> execute connectionString updateSyncHistorySql

    let addSyncHistory sourceId =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@SourceId" sourceId
        
        commandFunc |> execute connectionString addSyncHistorySql

    //let getLastSynched sourceId : DateTime option =
    //    sourceId
    //     |> readInSyn
    //     |> getSyncHistory
    //     |> List.tryHead
    //     |> function | Some info -> Some info.LastSynched
    //                 | None      -> None    


    let removeLinkTopic linkId =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@LinkId" linkId
        
        commandFunc |> execute connectionString deleteLinkTopicSql

    let follow (info:FollowRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@SubscriberId" (Int32.Parse(info.SubscriberId))
                    |> addWithValue "@ProfileId"    (Int32.Parse(info.ProfileId))
        
        commandFunc |> execute connectionString followSql

    let unsubscribe (info:UnsubscribeRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@SubscriberId" info.SubscriberId
                    |> addWithValue "@ProfileId"    info.ProfileId

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

    let featureTopics (info:FeaturedTopicsRequest) : string =
        let addFeaturedTopic name =

            let setFeatured topicId =
                featureTopic { ProfileId=  info.ProfileId
                               Name=       name
                               TopicId=    topicId
                               IsFeatured= true 
                             }

            match getTopic name with
            | Some topic -> setFeatured topic.Id
            | None       -> let topicId =  addTopic {Name= name}
                            setFeatured <| Int32.Parse(topicId)

        info.ProfileId |> clearFeaturedTopics |> ignore

        info.Names 
         |> List.map addFeaturedTopic
         |> String.concat ","
        

    let featureLink (info:FeatureLinkRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@Id"         info.LinkId
                    |> addWithValue "@IsFeatured" info.IsFeatured

        commandFunc |> execute connectionString featureLinkSql

    let updateProfile (info:ProfileRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@Id"        (Int32.Parse(info.Id))
                    |> addWithValue "@FirstName" info.FirstName
                    |> addWithValue "@LastName"  info.LastName
                    |> addWithValue "@bio"       info.Bio
                    |> addWithValue "@email"     info.Email

        commandFunc |> execute connectionString updateProfileSql

    let updateThumbnail (info:UpdateThumbnailRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@ProfileId" (Int32.Parse(info.ProfileId))
                    |> addWithValue "@ImageUrl"   info.ImageUrl

        commandFunc |> execute connectionString updateThumbnailSql

    let private dataSourceToPlatformUser (info:DataSourceRequest) =

        let apikey = info.Platform |> platformFromString |> getKey

        { ProfileId=  info.ProfileId
          Platform=   info.Platform |> platformFromString
          APIKey=     apikey
          User=     { AccessId = info.AccessId; ProfileId= info.ProfileId }
        }

    let updateSourceRequest info links =
        
        let linkIds = links |> Seq.map addLink |> Seq.toList
        let zipped =  Seq.zip links linkIds

        let updatedLinks = 
            zipped |> Seq.map (fun linkAndId -> 
                                let link = fst linkAndId
                                let id   = snd linkAndId
                                { link with Id = Int32.Parse(id) })

        { info with Links= updatedLinks }

    let addDataSource (info:DataSourceRequest) =
    
        let links =         info  |> dataSourceToPlatformUser |> platformLinks
        let pendingSource = links |> updateSourceRequest info
        let sourceIdText =  addSource pendingSource
        let sourceId =      Int32.Parse(sourceIdText)

        let updatedSource = { pendingSource with Id = sourceId }

        sourceId
         |> getLastSynched 
         |> function
            | Some _ -> updateSyncHistory sourceId |> ignore
            | None   -> addSyncHistory    sourceId |> ignore

        pendingSource.Links 
         |> Seq.toList 
         |> List.iter (fun link -> addSourceLink updatedSource link |> ignore )

        sourceId |> string
        
    let syncDataSource (info:DataSourceRequest) =

        getLastSynched info.Id 
         |> function
            | Some lastSynched ->
                let updatedSource = info |> dataSourceToPlatformUser 
                                         |> newPlatformLinks lastSynched 
                                         |> updateSourceRequest info
                updatedSource.Links 
                 |> List.ofSeq 
                 |> List.iter (fun link -> addSourceLink updatedSource link |> ignore )

                info.Id  |> string

            | None -> info.Id  |> string

    let removeDataSource (info:RemoveDataSourceRequest) =
        
        let deleteSourceCommandFunc (command: SqlCommand) = 
            command |> addWithValue "@Id" info.Id

        let deleteSourceLinksCommandFunc (command: SqlCommand) = 
            command |> addWithValue "@SourceId" info.Id

        let deleteSyncHistoryCommandFunc (command: SqlCommand) = 
            command |> addWithValue "@SourceId" info.Id

        let deleteLinkTopics (links: Link list) = 
            links |> List.iter (fun l -> removeLinkTopic l.Id |> ignore)

        let deleteLinks (links: Link list) = 
            links |> List.iter (fun l -> removeLink { LinkId= l.Id } |> ignore)
        
        let links = linksFromSource info.Id getSourceLinksBySourceIdSql
        
        deleteSyncHistoryCommandFunc |> execute connectionString deleteSyncHistorySql |> ignore
        deleteSourceLinksCommandFunc |> execute connectionString deleteSourceLinksSql |> ignore

        links |> deleteLinkTopics
        links |> deleteLinks

        deleteSourceCommandFunc |> execute connectionString deleteSourceSql