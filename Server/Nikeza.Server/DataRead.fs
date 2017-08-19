module Nikeza.Server.DataRead

open System
open System.Data.SqlClient
open Nikeza.Server.Models

let dispose (connection:SqlConnection) (command:SqlCommand) =
    connection.Dispose()
    command.Dispose()

let addWithValue paramName obj (command: SqlCommand) =
    command.Parameters.AddWithValue(paramName,  obj) |> ignore
    command

let readCommand (connection: SqlConnection) (command: SqlCommand) readerFunc =
    if connection.State = System.Data.ConnectionState.Closed
    then connection.Open()

    let reader = command.ExecuteReader()
    let data = seq { while reader.Read() do yield readerFunc(reader) }
    connection.Close() |> ignore
    data

let createCommand sql connectionString =
    let connection = new SqlConnection(connectionString)
    let command =    new SqlCommand(sql,connection)
    (connection, command)

let sqlReader (reader: SqlDataReader) = { 
    ProfileId =    reader.["Id"].ToString() |> int
    FirstName =    reader.["FirstName"].ToString()
    LastName =     reader.["LastName"].ToString()
    Email =        reader.["Email"].ToString()
    ImageUrl =     reader.["ImageUrl"].ToString()
    Bio =          reader.["Bio"].ToString()
    PasswordHash = reader.["PasswordHash"].ToString()
    Salt =         reader.["Salt"].ToString()
    Created =      DateTime.Parse(reader.["Created"].ToString()) 
}

let rec readInLinks links (reader:SqlDataReader) =

    if reader.Read() then
    
        let link = { 
              Id =            reader.GetInt32  (0)
              ProviderId =    reader.GetInt32  (1)
              Title =         reader.GetString (2)
              Description =   reader.GetString (3)
              Url =           reader.GetString (4)
              ContentType =   reader.GetInt32  (5) |> contentTypeIdToString
              IsFeatured =    reader.GetBoolean(6)
        }
        readInLinks (link::links) reader

    else links

let rec readInProfiles profiles (reader:SqlDataReader) =

    if reader.Read() then
    
        let profile : ProfileRequest = {
            ProfileId=  reader.GetInt32 (0)
            FirstName=  reader.GetString(1)
            LastName=   reader.GetString(2)
            Email=      reader.GetString(3)
            ImageUrl=   reader.GetString(4)
            Bio=        reader.GetString(5)
        }

        readInProfiles (profile::profiles) reader
    else profiles

let rec readInSources sources (reader:SqlDataReader) =

    if reader.Read()
    then let source : AddSourceRequest = {
            ProfileId= reader.GetInt32  (0)
            Platform=   reader.GetString(1)
            Username=   reader.GetString(2)
         }
         readInSources (source::sources) reader
    else sources

let rec readInPlatforms platforms (reader:SqlDataReader) =

    if   reader.Read() 
    then readInPlatforms (reader.GetString (0)::platforms) reader
    else platforms