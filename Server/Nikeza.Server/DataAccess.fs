module Nikeza.Server.DataAccess

open System
open Nikeza.Server.Models
open System.Data.SqlClient

[<Literal>]
let ConnectionString = @"Data Source=DESKTOP-GE7O8JT\SQLEXPRESS;Initial Catalog=Nikeza;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"

let addWithValue paramName obj (command: SqlCommand) =
    command.Parameters.AddWithValue(paramName,  obj) |> ignore
    command

let executeNonQuery (command: SqlCommand) = 
    command.ExecuteNonQuery() |> ignore 
    command

let findUser email passwordHash =
    let query = "SELECT * FROM Profile Where Email = @email AND PasswordHash = @hash"
    use connection = new SqlConnection(ConnectionString)

    use command = (new SqlCommand(query,connection)
        |> addWithValue "@email" email 
        |> addWithValue "@hash"  passwordHash
    )

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

    use connection = new SqlConnection(ConnectionString)
    use command =    (new SqlCommand(sql,connection)
        |> addWithValue "@ProfileId"  info.SubscriberId
        |> addWithValue "@ProviderId" info.ProviderId
    )
   
    command.ExecuteNonQuery() |> ignore

let private unsubscribe(info:UnsubscribeRequest) =
    let sql = @"DELETE FROM [dbo].[Subscription]
                WHERE ProfileId  = @ProfileId AND
                      ProviderId = @ProviderId"

    use connection = new SqlConnection(ConnectionString)
    (new SqlCommand(sql,connection)
        |> addWithValue "@ProfileId"  info.SubscriberId
        |> addWithValue "@ProviderId" info.ProviderId
    )
    |> executeNonQuery
    |> ignore

let private featureLink (info:FeatureLinkRequest) =
    let sql = @"UPDATE [dbo].[Link]
                SET    [IsFeatured] = @IsFeatured
                WHERE  Id = @Id"

    use connection = new SqlConnection(ConnectionString)
    (new SqlCommand(sql,connection)
        |> addWithValue "@Id"         info.LinkId
        |> addWithValue "@IsFeatured" info.IsFeatured
    )
    |> executeNonQuery
    |> ignore

let private updateProfile (info:UpdateProfileRequest) =
    let sql = @"UPDATE [dbo].[Provider]
                SET    [Bio] =   @bio
                       [Email] = @email
                WHERE  Id =      @Id"

    use connection = new SqlConnection(ConnectionString)
    (new SqlCommand(sql,connection)
        |> addWithValue "@Id"    info.ProviderId
        |> addWithValue "@bio"   info.Bio
        |> addWithValue "@email" info.Email
    )
    |> executeNonQuery
    |> ignore

let execute = function
    | Follow        info -> follow        info
    | Unsubscribe   info -> unsubscribe   info
    | FeatureLink   info -> featureLink   info
    | UpdateProfile info -> updateProfile info