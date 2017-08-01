module Nikeza.Server.DB

open System
open Nikeza.Server.Models
open System.Data.SqlClient

let findUser (connString: string) email passwordHash =
    let query = "SELECT * FROM Profile Where Email = @email AND PasswordHash = @hash"
    use connection = new SqlConnection(connString)

    use command = new SqlCommand(query,connection)
    command.Parameters.AddWithValue("@email", email) |> ignore
    command.Parameters.AddWithValue("@hash", passwordHash) |> ignore

    connection.Open()
    let reader = command.ExecuteReader()
    let profiles = 
        seq {
            while reader.Read() do 
                yield { 
                    ProfileId = reader.["Id"].ToString() |> int
                    FirstName = reader.["FirstName"].ToString()
                    LastName = reader.["LastName"].ToString()
                    Email = reader.["Email"].ToString()
                    ImageUrl = reader.["ImageUrl"].ToString()
                    Bio = reader.["Bio"].ToString()
                    Created = DateTime.Parse(reader.["Created"].ToString()) 
                }
        }
        
    profiles 
    |> Seq.tryHead

let follow request =      () // TO BE IMPLEMENTED
let unsubscribe request = () // TO BE IMPLEMENTED
let featureLink request = () // TO BE IMPLEMENTED