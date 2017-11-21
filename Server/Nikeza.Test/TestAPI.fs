module Nikeza.Server.TestAPI

open System
open System.IO
open System.Data
open System.Data.SqlClient
open Sql
open Read
open Literals
open DatabaseCommand
open Model

[<Literal>]
let ChannelIdFile = @"C:\Nikeza\YouTube_ChannelId.txt"

let someStackoverflowImage = "https://www.gravatar.com/avatar/189471ba701cd80f6dfec2f8db53f7a8?s=128&d=identicon&r=PG"
let someYoutubeImage =       "https://yt3.ggpht.com/-AblAm4NIM8k/AAAAAAAAAAI/AAAAAAAAAAA/W-4O4xNRYVk/s88-c-k-no-mo-rj-c0xffffff/photo.jpg"
let someWordpressImage =     "https://1.gravatar.com/avatar/189471ba701cd80f6dfec2f8db53f7a8?s=96&d=identicon&r=G"
let someMediumImage =        "https://cdn-images-1.medium.com/fit/c/150/150/1*5Ov1SzK7a_XUDlCc42yIGA@2x.jpeg"

let stackoverflowUserId = "492701"
let mediumUserId =        "mike"
let wordpressUserId =     "bizmonger.wordpress.com"
let rssFeedId =           "http://www.pwop.com/feed.aspx?show=dotnetrocks&filetype=master&tags=F%23"
let youtubeUserId =       File.ReadAllText(ChannelIdFile)

let prepareReader (command:SqlCommand) =
    let reader = command.ExecuteReader()
    reader.Read() |> ignore
    reader

let someProfileId =    0
let someSubscriberId = 1
let someLinkId =       0
let someSourceId =     0


let someTopic = {
    Id= -1
    Name= "SomeTopic"
}

let someProviderTopic = {
    Id= -1
    Name= "SomeTopic"
    IsFeatured= false
}

let someLink = {
    Id=         0
    ProfileId=    someProfileId |> string
    Title=       "some_title"
    Description= "some_description"
    Url=         "some_url.com"
    Topics=       []
    ContentType=  ArticleText
    IsFeatured=   false
}

let someProfile = { 
    Id =     someProfileId |> string
    FirstName =     "Scott"
    LastName =      "Nimrod"
    Email =         "abc@abc.com"
    ImageUrl =      "some_url_.com"
    Bio =           "Some Bio"
    Sources =        []
    PasswordHash =  "XXX"
    Salt =          "XXX"
    Created =       DateTime.Now
}

let emptyProfile = { 
    Id =            ""
    FirstName =     ""
    LastName =      ""
    Email =         ""
    ImageUrl =      ""
    Bio =           ""
    Sources =        []
    PasswordHash =  ""
    Salt =          ""
    Created =       DateTime.Now
}

let registerProfile registrationForm =
    Registration.register registrationForm |> function
    | Success profile -> profile.Id
    | Failure         -> emptyProfile.Id

let (someRegistrationForm:RegistrationRequest) = { 
    FirstName = "Ace"
    LastName =  "Thomas"
    Email =     "ace@abc.com"
    Password =  "123"
}

let (someSubscriberRegistrationForm:RegistrationRequest) = { 
    FirstName = "Subscriber"
    LastName =  "Doe"
    Email =     "subscriber@abc.com"
    Password =  "123"
}

let someSource = {
    Id=         0
    ProfileId = someProfileId |> string
    Platform = "YouTube"
    AccessId =  File.ReadAllText(ChannelIdFile)
    Links =     []
}

let someUpdatedProfile: ProfileRequest = { 
    Id = someProfileId |> string
    FirstName = "Scott"
    LastName =  "Nimrod"
    Email =     "abc@abc.com"
    ImageUrl =  "someUrl.com"
    Bio =       "Some Bio"
    Sources =    []
}

let someSubscriber: Profile = { 
    Id =     someSubscriberId |> string
    FirstName =     "Subscriber"
    LastName =      "Doe"
    Email =         "subscriber@abc.com"
    ImageUrl =      "some_url_.com"
    Bio =           "Some Bio"
    Sources =        []
    PasswordHash =  "XXX"
    Salt=           "XXX"
    Created =       DateTime.Now
}

let executeCommand sql =
    let (connection,command) = createCommand sql connectionString

    if connection.State = ConnectionState.Closed
    then connection.Open()

    command.ExecuteNonQuery()  |> ignore
    dispose connection command

let cleanDataStore() =
    executeCommand @"DELETE FROM ObservedLinks"
    executeCommand @"DELETE FROM SourceLinks"
    executeCommand @"DELETE FROM LinkTopic"
    executeCommand @"DELETE FROM Link"
    executeCommand @"DELETE FROM FeaturedTopic"
    executeCommand @"DELETE FROM Topic"
    executeCommand @"DELETE FROM Source"
    executeCommand @"DELETE FROM Subscription"
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