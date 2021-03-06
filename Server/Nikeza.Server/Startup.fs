module Nikeza.Server.Startup

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.StaticFiles
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open Giraffe.HttpHandlers
open Giraffe.Middleware
open Giraffe.Razor.Middleware
open Nikeza.Server.Routes
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Authentication.Cookies
open Routes


// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(EventId(0), ex, "An unhandled exception has occurred while executing the request.")
    clearResponse
    >=> setStatusCode 500
    >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("*")
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials() |> ignore

let cookieAuth (o : CookieAuthenticationOptions) =
    do
        o.Cookie.HttpOnly     <- true
        o.Cookie.SecurePolicy <- CookieSecurePolicy.SameAsRequest
        o.SlidingExpiration   <- true
        o.ExpireTimeSpan      <- TimeSpan.FromDays 7.0

let configureApp (app : IApplicationBuilder) =
    app.UseCors configureCors |> ignore
    app.UseGiraffeErrorHandler errorHandler |> ignore
    app.UseDefaultFiles()   |> ignore
    app.UseStaticFiles()    |> ignore
    app.UseAuthentication() |> ignore
    app.UseGiraffe webApp
 
let configureServices (services : IServiceCollection) =

    let serviceProvider  = services.BuildServiceProvider()
    let environment =      serviceProvider.GetService<IHostingEnvironment>()
    let viewsFolderPath =  IO.Path.Combine(environment.ContentRootPath, "Views")
    
    services.AddRazorEngine (viewsFolderPath) |> ignore
    services.AddAuthentication(authScheme)
            .AddCookie(fun o -> o |> cookieAuth) |> ignore
    services.AddCors() |> ignore // Enables CORS

let configureLogging (loggerFactory : ILoggerFactory) =
    loggerFactory.AddConsole(LogLevel.Trace).AddDebug() |> ignore