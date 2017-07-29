module Nikeza.Server.Routes

open System
open Microsoft.AspNetCore.Http
open Giraffe.HttpContextExtensions
open Giraffe.HttpHandlers
open Nikeza.Server.Models.Authentication
open Nikeza.Server.Models
open Nikeza.YouTube.Data
open Nikeza.YouTube.Data.Authentication
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

let webApp : HttpContext -> HttpHandlerResult = 

    choose [
        GET >=>
            choose [
                route "/" >=> htmlFile "/hostingstart.html"
                //route "/" >=> htmlFile "/home.html"
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