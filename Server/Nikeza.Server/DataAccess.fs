module Nikeza.Server.DataAccess

open System
open Nikeza.Server.Models
open System.Data.SqlClient

[<Literal>]
let ConnectionString = @"Data Source=DESKTOP-GE7O8JT\SQLEXPRESS;Initial Catalog=Nikeza;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"

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
                    ProfileId = reader.["Id"].ToString() |> int
                    FirstName = reader.["FirstName"].ToString()
                    LastName =  reader.["LastName"].ToString()
                    Email =     reader.["Email"].ToString()
                    ImageUrl =  reader.["ImageUrl"].ToString()
                    Bio =       reader.["Bio"].ToString()
                    Created =   DateTime.Parse(reader.["Created"].ToString()) 
                }
        }
        
    profiles |> Seq.tryHead

let private follow (info:FollowRequest) =
    let sql = @"INSERT INTO [dbo].[Subscription]
                        ([ProfileId]
                        ,[ProviderId])
                  VALUES
                         @ProfileId
                        ,@ProviderId"

    let (connection,command) = createCommand(sql)

    command.Parameters.AddWithValue("@ProfileId",  info.SubscriberId) |> ignore
    command.Parameters.AddWithValue("@ProviderId", info.ProviderId)   |> ignore
    command.ExecuteNonQuery() |> ignore

    dispose connection command

let private unsubscribe(info:UnsubscribeRequest) =
    let sql = @"DELETE FROM [dbo].[Subscription]
                WHERE ProfileId  = @ProfileId AND
                      ProviderId = @ProviderId"

    let (connection,command) = createCommand(sql)

    command.Parameters.AddWithValue("@ProfileId",  info.SubscriberId) |> ignore
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
    | Follow        info -> follow        info
    | Unsubscribe   info -> unsubscribe   info
    | FeatureLink   info -> featureLink   info
    | UpdateProfile info -> updateProfile info