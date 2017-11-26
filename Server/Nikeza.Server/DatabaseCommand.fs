module Nikeza.Server.DatabaseCommand

open System
open System.Data.SqlClient
open Model
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
        let linkTopicId =
            let linkTopicsCommandFunc (command: SqlCommand) =
                command |> addWithValue "@LinkId"  linkTopic.Link.Id
                        |> addWithValue "@TopicId" linkTopic.Topic.Id

            linkTopicsCommandFunc |> execute connectionString addLinkTopicSql
        linkTopicId

    let addTopic (info:TopicRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@Name" info.Name
        
        commandFunc |> execute connectionString addTopicSql


    let featureTopic (info:FeatureTopicRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@ProfileId"  info.ProfileId
                    |> addWithValue "@TopicId"    info.TopicId

        commandFunc |> execute connectionString featureTopicSql

    let unfeatureTopic (info:FeatureTopicRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@ProfileId"  info.ProfileId
                    |> addWithValue "@TopicId"    info.TopicId

        commandFunc |> execute connectionString unfeatureTopicSql


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

        let addTopic (providerTopic:ProviderTopic) =
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

    let addDataSource (info:DataSourceRequest) =

        let apikey = info.Platform |> platformFromString |> getKey
        let platformUser = {
            ProfileId=  info.ProfileId
            Platform=   info.Platform |> platformFromString
            APIKey=     apikey
            User=     { AccessId = info.AccessId; ProfileId= info.ProfileId }
        }

        let links =   linksFrom platformUser
        let linkIds = links |> Seq.map addLink |> Seq.toList
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
                     |> List.iter (fun link -> addSourceLink updatedSource link |> ignore ) 
        sourceId

    let removeDataSource (info:RemoveDataSourceRequest) =
        let commandFunc (command: SqlCommand) = 
            command |> addWithValue "@Id" info.Id
            
        commandFunc |> execute connectionString deleteSourceSql