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


// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) (context : HttpContext) =
    logger.LogError(EventId(0), ex, "An unhandled exception has occurred while executing the request.")
    context |> (clearResponse >=> setStatusCode 500 >=> text ex.Message)

// ---------------------------------
// Config and Main
// ---------------------------------
let cookieAuth =
    CookieAuthenticationOptions(
            AuthenticationScheme    = authScheme,
            AutomaticAuthenticate   = true,
            AutomaticChallenge      = false,
            CookieHttpOnly          = true,
            CookieSecure            = CookieSecurePolicy.SameAsRequest,
            SlidingExpiration       = true,
            ExpireTimeSpan          = TimeSpan.FromDays 7.0
    )

let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader() |> ignore

let configureApp (app : IApplicationBuilder) =
    app.UseCors configureCors |> ignore
    app.UseGiraffeErrorHandler errorHandler
    app.UseCookieAuthentication cookieAuth |> ignore
    app.UseStaticFiles() |> ignore
    app.UseGiraffe webApp
 
let configureServices (services : IServiceCollection) =
    let serviceProvider  = services.BuildServiceProvider()
    let environment = serviceProvider.GetService<IHostingEnvironment>()
    let viewsFolderPath = IO.Path.Combine(environment.ContentRootPath, "Views")
    services.AddRazorEngine viewsFolderPath |> ignore
    services.AddAuthentication() |> ignore
    services.AddCors |> ignore // Enables CORS

let configureLogging (loggerFactory : ILoggerFactory) =
    loggerFactory.AddConsole(LogLevel.Trace).AddDebug() |> ignore