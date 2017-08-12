module Nikeza.Server.DataStore

open System
open Nikeza.Server.Models
open System.Data.SqlClient

let connectionString = "Data Source=DESKTOP-GE7O8JT\\SQLEXPRESS;Initial Catalog=Nikeza;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"

let dispose (connection:SqlConnection) (command:SqlCommand) =
    connection.Dispose()
    command.Dispose()
    
module private Store = 
    let private executeNonQuery (command: SqlCommand) = command.ExecuteNonQuery() |> ignore
    let private executeQuery (command: SqlCommand) = command.ExecuteReader()

    let createOpenConnection connectionString =
        let connection = new SqlConnection(connectionString)
        connection.Open()
        connection

    let execute connectionString sql commandFunc = 
        let connection = createOpenConnection connectionString

        use command = (new SqlCommand(sql,connection)) |> commandFunc
        executeNonQuery command
        dispose connection command

    let query connectionString sql commandFunc = 
        let connection = createOpenConnection connectionString
        use command = (new SqlCommand(sql,connection)) |> commandFunc
        let results = executeQuery command
        dispose connection command
        results

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
    | "article" -> 0
    | "video"   -> 1
    | "answer"  -> 2
    | "podcast" -> 3
    | _         -> -1

let contentTypeToString = function
    | Article -> "article"
    | Video   -> "video"  
    | Answer  -> "answer" 
    | Podcast -> "podcast"
    | Unknown -> "unknown"        

let addWithValue paramName obj (command: SqlCommand) =
    command.Parameters.AddWithValue(paramName,  obj) |> ignore
    command

let readCommand (connection: SqlConnection) (command: SqlCommand) readerFunc =
    if connection.State = System.Data.ConnectionState.Closed
    then connection.Open()

    let reader = command.ExecuteReader()
    let data = seq {
        while reader.Read() do yield readerFunc(reader)
    }
    connection.Close() |> ignore
    data

let executeNonQuery (command: SqlCommand) = command.ExecuteNonQuery() |> ignore

let createCommand sql =
    let connection = new SqlConnection(connectionString)
    connection.Open()

    let command = new SqlCommand(sql,connection)
    (connection, command)

let findUser email passwordHash: (Profile option) =
    let query = "SELECT * FROM Profile Where Email = @email AND PasswordHash = @hash"
    use connection = new SqlConnection(connectionString)

    use command = new SqlCommand(query,connection)
    command
    |> addWithValue "@email"  email
    |> addWithValue "@hash"   passwordHash
    |> ignore

    let sqlReader (reader: SqlDataReader) = { 
        ProfileId =    reader.["Id"].ToString() |> int
        FirstName =    reader.["FirstName"].ToString()
        LastName =     reader.["LastName"].ToString()
        Email =        reader.["Email"].ToString()
        ImageUrl =     reader.["ImageUrl"].ToString()
        Bio =          reader.["Bio"].ToString()
        PasswordHash = reader.["PasswordHash"].ToString()
        Created =      DateTime.Parse(reader.["Created"].ToString()) 
    }

    readCommand connection command sqlReader |> Seq.tryHead
    

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

    command
    |> addWithValue "@FirstName"     info.FirstName
    |> addWithValue "@LastName"      info.LastName
    |> addWithValue "@Email"         info.Email
    |> addWithValue "@ImageUrl"      info.ImageUrl
    |> addWithValue "@Bio"           info.Bio
    |> addWithValue "@PasswordHash"  info.PasswordHash
    |> addWithValue "@Created"       info.Created
    |> executeNonQuery

    dispose connection command

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

    command 
    |> addWithValue "@ProviderId"    info.ProviderId
    |> addWithValue "@Title"         info.Title
    |> addWithValue "@Description"   info.Description
    |> addWithValue "@Url"           info.Url
    |> addWithValue "@ContentTypeId" (info.ContentType |> contentTypeToId)
    |> addWithValue "@IsFeatured"    info.IsFeatured
    |> addWithValue "@Created"       DateTime.Now
    |> executeNonQuery

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

    command
    |> addWithValue "@SubscriberId"  info.SubscriberId
    |> addWithValue "@ProviderId"    info.ProviderId
    |> executeNonQuery

    dispose connection command

let private unsubscribe (info:UnsubscribeRequest) =
    let sql = @"DELETE FROM [dbo].[Subscription]
                WHERE SubscriberId  = @SubscriberId AND
                      ProviderId =    @ProviderId"
    let commandFunc (command: SqlCommand) = 
        command 
        |> addWithValue "@SubscriberId"  info.SubscriberId
        |> addWithValue "@ProviderId"    info.ProviderId

    Store.execute connectionString sql commandFunc

let private featureLink (info:FeatureLinkRequest) =
    let sql = @"UPDATE [dbo].[Link]
                SET    [IsFeatured] = @IsFeatured
                WHERE  Id = @Id"

    let (connection,command) = createCommand(sql)

    command
    |> addWithValue "@Id"          info.LinkId
    |> addWithValue "@IsFeatured"  info.IsFeatured
    |> executeNonQuery

    dispose connection command

let private updateProfile (info:UpdateProfileRequest) =
    let sql = @"UPDATE [dbo].[Profile]
                SET    [FirstName] = @FirstName,
                       [LastName] =  @LastName,
                       [Bio] =       @bio,
                       [Email] =     @email
                WHERE  Id =          @Id"

    let (connection,command) = createCommand(sql)

    command
    |> addWithValue "@Id"        info.ProviderId
    |> addWithValue "@FirstName" info.FirstName
    |> addWithValue "@LastName"  info.LastName
    |> addWithValue "@bio"       info.Bio
    |> addWithValue "@email"     info.Email
    |> executeNonQuery

    dispose connection command

let private getLinks profileId =
    let sql = "SELECT Id, 
                      ProviderId, 
                      Title, 
                      Description, 
                      Url, 
                      ContentTypeId, 
                      IsFeatured, 
                      Created

               FROM   [dbo].[Link]
               WHERE  ProviderId = @ProviderId"

    let commandFunc (command: SqlCommand) = 
        command |> addWithValue "@ProviderId" profileId

    let reader = Store.query connectionString sql commandFunc
    seq []

let private getFollowers     profileId = seq []
let private getSubscriptions profileId = seq []

let execute = function
    | Register      info -> register      info
    | UpdateProfile info -> updateProfile info
    | Follow        info -> follow        info
    | Unsubscribe   info -> unsubscribe   info
    | AddLink       info -> addLink       info
    | FeatureLink   info -> featureLink   info

let respondTo = function
    | GetLinks         info -> getLinks         info.ProviderId
    | GetFollowers     info -> getFollowers     info.ProviderId // TODO
    | GetSubscriptions info -> getSubscriptions info.ProviderId // TODO
