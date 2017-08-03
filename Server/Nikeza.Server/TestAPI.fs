module Nikeza.TestAPI

open System.Data.SqlClient
open Nikeza.Server.DataAccess

let createCommand sql =
    use connection = new SqlConnection(ConnectionString)
    new SqlCommand(sql,connection)

let prepareReader (command:SqlCommand) =
    let reader = command.ExecuteReader()
    reader.Read() |> ignore
    reader