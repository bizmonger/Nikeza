module Nikeza.TestAPI

open System
open System.Data.SqlClient
open Nikeza.Server.Sql
open Nikeza.Server.Read
open Nikeza.Server.Literals
open Nikeza.Server.Command
open Nikeza.Server.Store
open Nikeza.Server.Model

let prepareReader (command:SqlCommand) =
    let reader = command.ExecuteReader()
    reader.Read() |> ignore
    reader

let someProfileId =   0
let someSubscriberId = 1
let someLinkId =       0
let someSourceId =     0

let someLink = {
    Id=         0
    ProfileId=  someProfileId |> string
    Title=       "some_title"
    Description= "some_description"
    Url=         "some_url.com"
    Topics=      []
    ContentType= ArticleText
    IsFeatured=  false
}

let someProfile = { 
    ProfileId =     someProfileId |> string
    FirstName =     "Scott"
    LastName =      "Nimrod"
    Email =         "abc@abc.com"
    ImageUrl =      "some_url_.com"
    Bio =           "Some Bio"
    Sources =       []
    PasswordHash =  "XXX"
    Salt =          "XXX"
    Created =       DateTime.Now
}

let someSource = {
    Id=         0
    ProfileId = someProfileId |> string
    Platform = "YouTube"
    AccessId = "some_access_id"
    Links =     []
    APIKey=    "some_api_key"
}

let someUpdatedProfile: ProfileRequest = { 
    ProfileId = someProfileId |> string
    FirstName = "Scott"
    LastName =  "Nimrod"
    Email =     "abc@abc.com"
    ImageUrl =  "someUrl.com"
    Bio =       "Some Bio"
    Sources =   []
}

let someSubscriber: Profile = { 
    ProfileId =     someSubscriberId |> string
    FirstName =     "Scott"
    LastName =      "Nimrod"
    Email =         "abc@abc.com"
    ImageUrl =      "some_url_.com"
    Bio =           "Some Bio"
    Sources =       []
    PasswordHash =  "XXX"
    Salt=           "XXX"
    Created =       DateTime.Now
}

let executeCommand sql =
    let (connection,command) = createCommand sql connectionString

    if connection.State = System.Data.ConnectionState.Closed
    then connection.Open()

    command.ExecuteNonQuery()  |> ignore
    dispose connection command

let cleanDataStore() =
    executeCommand @"DELETE FROM Link"
    executeCommand @"DELETE FROM Topic"
    executeCommand @"DELETE FROM Source"
    executeCommand @"DELETE FROM Subscription"
    executeCommand @"DELETE FROM ProfileTopics"
    executeCommand @"DELETE FROM ProviderSources"
    executeCommand @"DELETE FROM Profile"

let cleanup command connection =
    dispose connection command
    cleanDataStore()

let getLastId tableName =

    let rec getIds (reader:SqlDataReader) ids dataAvailable =

        if    dataAvailable
        then  let ids' = reader.GetInt32(0)::ids
              getIds reader ids' (reader.Read())

        else  ids

    let sql = @"SELECT Id From " + tableName
    let (connection,command) = createCommand sql connectionString
    connection.Open()
    let reader = command.ExecuteReader()

    let ids = getIds reader [] (reader.Read())
    dispose connection command |> ignore
    ids |> List.max