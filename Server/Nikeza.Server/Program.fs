module Nikeza.Server.App

open System
open System.IO
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Nikeza.Server.Startup

[<EntryPoint>]
let main argv =              
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot = Path.Combine(contentRoot, "wwwroot")                  
    WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseIISIntegration()
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(Action<IServiceCollection> configureServices)
        .ConfigureLogging(Action<ILoggerFactory> configureLogging)
        // '0.0.0.0' must be used since 'localhost' does not work in docker.
        // Port 5000 doesn't work in an Azure Deployment
        //.UseUrls("http://0.0.0.0:5000") 
        .Build()
        .Run()
    0