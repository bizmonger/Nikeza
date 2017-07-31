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
    fun(context: HttpContext) -> 
        async {
            let! data = context.BindJson<RegistrationRequest>()
            let response = register data |> function
                                         | Success -> "Registered"
                                         | Failure -> "Not Registered"
            return! text response context 
        }

            
let loginHandler  (authFailedHandler : HttpHandler) = 
    fun(context: HttpContext) -> 
        async {
            let! data = context.BindJson<LogInRequest>()
            if  authenticate data.UserName data.Password
                then let user = getUserClaims data.UserName authScheme
                     do! context.Authentication.SignInAsync(authScheme, user) |> Async.AwaitTask 
                     return Some context 
                else return! authFailedHandler context                                                           
        } 

let setCode (handler:HttpHandler)= 
    fun(context: HttpContext) ->     
        let response =
             if context.Response.Body.ToString() = ""
                then setStatusCode 401
                else handler
        response context


let fetchYoutube (apiKey, channelId) (context : HttpContext) = 
    async {
        let  youtube = youTubeService apiKey
        let! videos =  uploadList youtube <| ChannelId channelId
        return! json videos context
    }

let fetchWordpress (feedUrl) (context : HttpContext) =
    async {
        let! response = jsonRssFeed feedUrl
        return! json response context
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
                route "/login"    >=> loginHandler (setStatusCode 401 >=> text "invalid credentials")
                route "/logout"   >=> signOff authScheme >=> text "logged out"
            ]
        setStatusCode 404 >=> text "Not Found" ]