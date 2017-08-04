module Nikeza.TestAPI

open System.Data.SqlClient
open Nikeza.Server.DataAccess

let prepareReader (command:SqlCommand) =
    let reader = command.ExecuteReader()
    reader.Read() |> ignore
    reader