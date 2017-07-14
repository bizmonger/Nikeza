module Nikeza.Server.App

open System
open System.IO
open System.Collections.Generic
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Http.Features
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe.HttpContextExtensions
open Giraffe.HttpHandlers
open Giraffe.Middleware
open Giraffe.Razor.HttpHandlers
open Giraffe.Razor.Middleware
open Nikeza.Server.Models.Authentication
open Nikeza.Server.Models
open Nikeza.Server.YouTube
open Nikeza.YouTube.Data
open Nikeza.YouTube.Data.Authentication
open Nikeza.Wordpress.Rss.Data
open Nikeza.Server.Wordpress

// ---------------------------------
// Web app
// ---------------------------------
let authScheme = "Cookie"
let registrationHandler = 
    fun(ctx: HttpContext) -> 
        async {
            let! data = ctx.BindJson<RegistrationRequest>()
            let status = register data
            let response = match status with 
                            | Success -> "Registered"
                            | Failure -> "Not Registered"
            return! text response ctx 
        }

            
let loginHandler  (authFailedHandler : HttpHandler) = 
    fun(ctx: HttpContext) -> 
        async {
            let! data = ctx.BindJson<LogInRequest>()
            let isAuthenticated = authenticate data.UserName data.Password
            if isAuthenticated
            then
                let user = getUserClaims data.UserName authScheme
                do! ctx.Authentication.SignInAsync(authScheme, user) |> Async.AwaitTask 
                return Some ctx 
            else 
                return! authFailedHandler ctx                                                           
        } 

let setCode (handler:HttpHandler)= 
    fun(ctx: HttpContext) ->     
        let response =
             if ctx.Response.Body.ToString() = ""
                then setStatusCode 401
                else handler
        response ctx


let fetchYoutube (apiKey, channelId) (ctx : HttpContext) = 
    async {
        let youtube = youTubeService apiKey
        let! videos = uploadList youtube <| ChannelId channelId
        return! json videos ctx
    }

// Wordpress
let fetchWordpress (feedUrl) (ctx : HttpContext) =
    async {
        let! response = jsonRssFeed feedUrl
        return! json response ctx
    }

let webApp = 

    choose [
        GET >=>
            choose [
                route "/" >=> htmlFile "/home.html"
                routef "/youtube/%s/%s" fetchYoutube
                routef "/wordpress/%s"  fetchWordpress

            ]
        POST >=> 
            choose [
                route "/register" >=> registrationHandler 
                route "/login" >=> loginHandler (setStatusCode 401 >=> text "invalid credentials")
                route "/logout" >=> signOff authScheme >=> text "logged out"
            ]
        setStatusCode 404 >=> text "Not Found" ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) (ctx : HttpContext) =
    logger.LogError(EventId(0), ex, "An unhandled exception has occurred while executing the request.")
    ctx |> (clearResponse >=> setStatusCode 500 >=> text ex.Message)

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

let configureApp (app : IApplicationBuilder) = 
    app.UseGiraffeErrorHandler errorHandler
    app.UseCookieAuthentication cookieAuth |> ignore
    app.UseGiraffe webApp

let configureServices (services : IServiceCollection) =
    let sp  = services.BuildServiceProvider()
    let env = sp.GetService<IHostingEnvironment>()
    let viewsFolderPath = Path.Combine(env.ContentRootPath, "Views")
    services.AddAuthentication() |> ignore
    services.AddRazorEngine viewsFolderPath |> ignore

let configureLogging (loggerFactory : ILoggerFactory) =
    loggerFactory.AddConsole(LogLevel.Trace).AddDebug() |> ignore

[<EntryPoint>]
let main argv =              
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot = Path.Combine(contentRoot, "wwwroot")                  
    WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(Action<IServiceCollection> configureServices)
        .ConfigureLogging(Action<ILoggerFactory> configureLogging)
        // '0.0.0.0' must be used since 'localhost' does not work in docker.
        .UseUrls("http://0.0.0.0:5000") 
        .Build()
        .Run()
    0
