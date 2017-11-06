module Nikeza.Server.Read

open System
open System.Data.SqlClient
open Nikeza.Server.Model
open Nikeza.Server.Converters

let readCommand (connection: SqlConnection) (command: SqlCommand) readerFunc =
    if connection.State = Data.ConnectionState.Closed
    then connection.Open()

    let reader = command.ExecuteReader()
    seq { while reader.Read() do yield readerFunc(reader) }

let createCommand sql connectionString =
    let connection = new SqlConnection(connectionString)
    let command =    new SqlCommand(sql,connection)
    (connection, command)

let sqlReader (reader: SqlDataReader) = { 
    ProfileId =    reader.["Id"].ToString()
    FirstName =    reader.["FirstName"].ToString()
    LastName =     reader.["LastName"].ToString()
    Email =        reader.["Email"].ToString()
    ImageUrl =     reader.["ImageUrl"].ToString()
    Bio =          reader.["Bio"].ToString()
    Sources =      [] // Todo...
    PasswordHash = reader.["PasswordHash"].ToString()
    Salt =         reader.["Salt"].ToString()
    Created =      DateTime.Parse(reader.["Created"].ToString()) 
}

let rec readInLinks links (reader:SqlDataReader) = reader.Read() |> function

    | true ->
        let link : Link = { 
              Id=          reader.GetInt32  (0)
              ProfileId=   reader.GetInt32  (1) |> string
              Title=       reader.GetString (2)
              Description= reader.GetString (3)
              Url=         reader.GetString (4)
              Topics=      []
              ContentType= reader.GetInt32  (5) |> contentTypeIdToString
              IsFeatured=  reader.GetBoolean(6)
        }
        
        readInLinks (link::links) reader

    | false -> links

and readInTopic (reader:SqlDataReader) = {
  Id=   reader.GetInt32  (0)
  Name= reader.GetString (1) }

and readInLink (reader:SqlDataReader) = {
  Id=          reader.GetInt32  (0)
  ProfileId=   reader.GetInt32  (1) |> string
  Title=       reader.GetString (2)
  Description= reader.GetString (3)
  Topics=      []
  Url=         reader.GetString (4)
  IsFeatured=  reader.GetBoolean(5)
  ContentType= reader.GetString (6) 
}

let rec readInTopics topics (reader:SqlDataReader) = reader.Read() |> function
    | true -> let topic = reader |> readInTopic
              readInTopics (topic::topics) reader
    | false -> topics

let readInFeaturedTopic (reader:SqlDataReader) = {
    Name= reader.GetString(2)
}

let readInProfile (reader:SqlDataReader) = { 
    ProfileId=    reader.GetInt32 (0) |> string
    FirstName=    reader.GetString(1)
    LastName=     reader.GetString(2)
    Email=        reader.GetString(3)
    ImageUrl=     reader.GetString(4)
    Bio=          reader.GetString(5)
    PasswordHash= reader.GetString(6)
    Salt=         reader.GetString(7)
    Created=      reader.GetDateTime(8)
    Sources=      [] 
}

let rec readInProfiles profiles (reader:SqlDataReader) = reader.Read() |> function
    | true  -> let profile = reader |> readInProfile
               readInProfiles (profile::profiles) reader
    | false -> profiles

let rec readInFeaturedTopics topics (reader:SqlDataReader) = reader.Read() |> function
    | true  -> let topic = reader |> readInFeaturedTopic
               readInFeaturedTopics (topic::topics) reader
    | false -> topics

and readInProvider (reader:SqlDataReader) = { 
    Profile=       reader |> readInProfile |> toProfileRequest
    Topics=        []
    Portfolio=     reader |> readInLinks  [] |> toPortfolio
    RecentLinks=   []
    Subscriptions= []
    Followers=     []
}

let rec readInProviders providers (reader:SqlDataReader) = reader.Read() |> function
    | true  -> let provider = reader |> readInProvider
               readInProviders (provider::providers) reader
    | false -> providers

let rec readInSources sources (reader:SqlDataReader) = reader.Read() |> function
    | true -> 
        let source : DataSourceRequest = {
            Id=         reader.GetInt32 (0)
            ProfileId=  reader.GetInt32 (1) |> string
            Platform=   reader.GetString(2)
            AccessId=   reader.GetString(3)
            Links =     [] // Handled in a separate step
         }

        readInSources (source::sources) reader

    | false -> sources

let rec readInPlatforms platforms (reader:SqlDataReader) = reader.Read() |> function
    | true  -> readInPlatforms (reader.GetString (0)::platforms) reader
    | false -> platforms

let rec readInProfileId (reader:SqlDataReader) = reader.Read() |> function
    | true  -> reader.GetString (0)
    | false -> ""