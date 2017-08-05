module Nikeza.TestAPI

open System
open System.Data.SqlClient
open Nikeza.Server.DataAccess
open Nikeza.Server.Models

let prepareReader (command:SqlCommand) =
    let reader = command.ExecuteReader()
    reader.Read() |> ignore
    reader
let someProviderId = 0
let someSubscriberId = 1
let someLinkId = 0

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