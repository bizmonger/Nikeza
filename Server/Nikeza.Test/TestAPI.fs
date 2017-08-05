module Nikeza.TestAPI

open System
open System.Data.SqlClient
open Nikeza.Server.DataStore
open Nikeza.Server.Models

let prepareReader (command:SqlCommand) =
    let reader = command.ExecuteReader()
    reader.Read() |> ignore
    reader
let someProviderId = 0
let someSubscriberId = 1
let someLinkId = 0

let someLink = {
    ProviderId=  someProviderId
    Title=       "some_title"
    Description= "some_description"
    Url=         "some_url.com"
    IsFeatured=  false
    ContentType= "article"
}

let someProfile = { 
    ProfileId =     someProviderId
    FirstName =     "Scott"
    LastName =      "Nimrod"
    Email =         "abc@abc.com"
    ImageUrl =      "some_url_.com"
    Bio =           "Some Bio"
    PasswordHash =  "XXX"
    Created =       DateTime.Now
}

let execute sql =
    let (connection,command) = createCommand(sql)
    command.ExecuteNonQuery()  |> ignore
    dispose connection command

let cleanDataStore =
    execute @"DELETE FROM [dbo].[Link]"
    execute @"DELETE FROM [dbo].[Topic]"
    execute @"DELETE FROM [dbo].[Source]"
    execute @"DELETE FROM [dbo].[Subscription]"
    execute @"DELETE FROM [dbo].[ProfileLinks]"
    execute @"DELETE FROM [dbo].[ProfileTopics]"
    execute @"DELETE FROM [dbo].[ProviderSources]"
    execute @"DELETE FROM [dbo].[Profile]"

let cleanup command connection =
    dispose connection command
    cleanDataStore

let getLastId tableName =

    let rec getIds (reader:SqlDataReader) (ids: int list) =
        if reader.Read()
        then getIds reader (reader.GetInt32(0)::ids)
        else ids

    let sql = @"SELECT Id From @table"
    let (connection,command) = createCommand(sql)

    command.Parameters.AddWithValue("@table", tableName) |> ignore
    let reader = command.ExecuteReader()
    let idsFound = []

    dispose connection command |> ignore
    getIds  reader idsFound    |> List.max

    

    

