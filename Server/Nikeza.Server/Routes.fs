module Nikeza.Server.Routes

open System
open Microsoft.AspNetCore.Http
open Giraffe.HttpContextExtensions
open Giraffe.HttpHandlers
open Nikeza.Server.Models.Authentication
open Nikeza.Server.Models
open Nikeza.Server.YouTube.Data
open Nikeza.Server.YouTube.Data.Authentication
open Nikeza.Server.Wordpress
open Nikeza.Server.DataAccess

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

let followHandler = 
    fun(context: HttpContext) -> 
        async {
            let! data = context.BindJson<FollowRequest>()
            execute <| Follow data
            return Some context
        } 

let unsubscribeHandler = 
    fun(context: HttpContext) -> 
        async {
            let! data = context.BindJson<UnsubscribeRequest>()
            execute <| Unsubscribe data
            return Some context                  
        } 

let featureLinkHandler = 
    fun(context: HttpContext) -> 
        async {
            let! data = context.BindJson<FeatureLinkRequest>()
            execute <| FeatureLink data
            return Some context
        }

let updateProfileHandler = 
    fun(context: HttpContext) -> 
        async {
            let! data = context.BindJson<UpdateProfileRequest>()
            execute <| UpdateProfile data
            return Some context
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
                route "/register"      >=> registrationHandler 
                route "/login"         >=> loginHandler (setStatusCode 401 >=> text "invalid credentials")
                route "/logout"        >=> signOff authScheme >=> text "logged out"
                route "/follow"        >=> followHandler
                route "/unsubscribe"   >=> unsubscribeHandler
                route "/featurelink"   >=> featureLinkHandler
                route "/updateprofile" >=> updateProfileHandler
            ]
        setStatusCode 404 >=> text "Not Found" ]