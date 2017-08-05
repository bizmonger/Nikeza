module Nikeza.Server.DataStore

open System
open Nikeza.Server.Models
open System.Data.SqlClient

let ConnectionString = Configuration.ConnectionString

[<Literal>] 
let ARTICLE = 0

[<Literal>] 
let VIDEO = 1

[<Literal>] 
let ANSWER = 2
[<Literal>] 
let PODCAST = 3
[<Literal>] 
let UNKNOWN = 4

type Table = Link | Subscription | Profile

let createCommand sql =
    let connection = new SqlConnection(ConnectionString)
    connection.Open()

    let command = new SqlCommand(sql,connection)
    (connection, command)

let dispose (connection:SqlConnection) (command:SqlCommand) =
    connection.Dispose()
    command.Dispose()

let findUser email passwordHash =
    let query = "SELECT * FROM Profile Where Email = @email AND PasswordHash = @hash"
    use connection = new SqlConnection(ConnectionString)

    use command = new SqlCommand(query,connection)
    command.Parameters.AddWithValue("@email", email)       |> ignore
    command.Parameters.AddWithValue("@hash", passwordHash) |> ignore

    connection.Open()
    let reader = command.ExecuteReader()
    let profiles = 
        seq {
            while reader.Read() do 
                yield { 
                    ProfileId =    reader.["Id"].ToString() |> int
                    FirstName =    reader.["FirstName"].ToString()
                    LastName =     reader.["LastName"].ToString()
                    Email =        reader.["Email"].ToString()
                    ImageUrl =     reader.["ImageUrl"].ToString()
                    Bio =          reader.["Bio"].ToString()
                    PasswordHash = reader.["PasswordHash"].ToString()
                    Created =      DateTime.Parse(reader.["Created"].ToString()) 
                }
        }
        
    profiles |> Seq.tryHead

let private register (info:Profile) =

    let sql = @"INSERT INTO [dbo].[Profile]
                      ( FirstName
                      , LastName
                      , Email
                      , ImgUrl
                      , Bio
                      , PasswordHash
                      , Created )
                VALUES
                       ( @FirstName
                       , @LastName
                       , @Email
                       , @ImageUrl
                       , @Bio
                       , @PasswordHash
                       , @Created
                       )"

    let (connection,command) = createCommand(sql)

    command.Parameters.AddWithValue("@FirstName",    info.FirstName)    |> ignore
    command.Parameters.AddWithValue("@LastName",     info.LastName)     |> ignore
    command.Parameters.AddWithValue("@Email",        info.Email)        |> ignore
    command.Parameters.AddWithValue("@ImageUrl",     info.ImageUrl)     |> ignore
    command.Parameters.AddWithValue("@Bio",          info.Bio)          |> ignore
    command.Parameters.AddWithValue("@PasswordHash", info.PasswordHash) |> ignore
    command.Parameters.AddWithValue("@Created",      info.Created)      |> ignore

    command.ExecuteNonQuery() |> ignore

    dispose connection command


let toContentTypeId = function
    | "article" -> ARTICLE
    | "video"   -> VIDEO
    | "answer"  -> ANSWER
    | "podcast" -> PODCAST
    | _         -> UNKNOWN

let private addLink (info:AddLinkRequest) =
    let sql = @"INSERT INTO [dbo].[Link]
                      (ProviderId
                      ,Title
                      ,Description
                      ,Url
                      ,ContentTypeId
                      ,IsFeatured
                      ,Created)
                VALUES
                      (@ProviderId
                      ,@Title
                      ,@Description
                      ,@Url
                      ,@ContentTypeId
                      ,@IsFeatured
                      ,@Created)"

    let (connection,command) = createCommand(sql)

    command.Parameters.AddWithValue("@ProviderId",  info.ProviderId)    |> ignore
    command.Parameters.AddWithValue("@Title",       info.Title)         |> ignore
    command.Parameters.AddWithValue("@Description", info.Description)   |> ignore
    command.Parameters.AddWithValue("@Url",         info.Url)           |> ignore
    command.Parameters.AddWithValue("@ContentTypeId", (info.ContentType |> toContentTypeId)) 
                                                                        |> ignore 
    command.Parameters.AddWithValue("@IsFeatured",  info.IsFeatured)    |> ignore
    command.Parameters.AddWithValue("@Created",     DateTime.Now)       |> ignore
    command.ExecuteNonQuery() |> ignore

    dispose connection command

let private follow (info:FollowRequest) =
    let sql = @"INSERT INTO [dbo].[Subscription]
                      (SubscriberId
                      ,ProviderId)
                VALUES
                       ( @SubscriberId 
                       , @ProviderId
                       )"

    let (connection,command) = createCommand(sql)

    command.Parameters.AddWithValue("@SubscriberId", info.SubscriberId) |> ignore
    command.Parameters.AddWithValue("@ProviderId",   info.ProviderId)   |> ignore
    command.ExecuteNonQuery() |> ignore

    dispose connection command

let private unsubscribe(info:UnsubscribeRequest) =
    let sql = @"DELETE FROM [dbo].[Subscription]
                WHERE SubscriberId  = @SubscriberId AND
                      ProviderId =    @ProviderId"

    let (connection,command) = createCommand(sql)

    command.Parameters.AddWithValue("@SubscriberId",  info.SubscriberId) |> ignore
    command.Parameters.AddWithValue("@ProviderId", info.ProviderId)   |> ignore
    command.ExecuteNonQuery() |> ignore

    dispose connection command

let private featureLink (info:FeatureLinkRequest) =
    let sql = @"UPDATE [dbo].[Link]
                SET    [IsFeatured] = @IsFeatured
                WHERE  Id = @Id"

    let (connection,command) = createCommand(sql)

    command.Parameters.AddWithValue("@Id"     , info.LinkId)     |> ignore
    command.Parameters.AddWithValue("@IsFeatured", info.IsFeatured) |> ignore
    command.ExecuteNonQuery() |> ignore

    dispose connection command

let private updateProfile (info:UpdateProfileRequest) =
    let sql = @"UPDATE [dbo].[Provider]
                SET    [Bio] =   @bio
                       [Email] = @email
                WHERE  Id =      @Id"

    let (connection,command) = createCommand(sql)

    command.Parameters.AddWithValue("@Id" ,   info.ProviderId) |> ignore
    command.Parameters.AddWithValue("@bio",   info.Bio)        |> ignore
    command.Parameters.AddWithValue("@email", info.Email)      |> ignore
    command.ExecuteNonQuery() |> ignore

    dispose connection command

let execute = function
    | Register      info -> register      info
    | Follow        info -> follow        info
    | Unsubscribe   info -> unsubscribe   info
    | AddLink       info -> addLink       info
    | FeatureLink   info -> featureLink   info
    | UpdateProfile info -> updateProfile info