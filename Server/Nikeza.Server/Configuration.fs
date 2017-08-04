module Nikeza.Server.Configuration

open System
open System.IO
open Microsoft.Extensions.Configuration

let configFilePath =
    (Directory.GetCurrentDirectory(),"../../../appsettings.json")
    |> Path.Combine
    |> Path.GetFullPath

let Configuration = ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile(configFilePath, optional = false, reloadOnChange = true)
                        .AddEnvironmentVariables()
                        .Build()

let ConnectionString = Configuration.GetConnectionString("NikezaDb")