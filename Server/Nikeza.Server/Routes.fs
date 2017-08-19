module Nikeza.Server.Routes

open Microsoft.AspNetCore.Http
open Giraffe.HttpContextExtensions
open Giraffe.HttpHandlers
open Nikeza.Server.Authentication
let authScheme = "Cookie"
let private registrationHandler = 
    fun(context: HttpContext) -> 
        async {
            let! data = context.BindJson<RegistrationRequest>()
            let response = register data |> function
                                         | Success -> "Registered"
                                         | Failure -> "Not Registered"
            return! text response context 
        }

let private loginHandler  (authFailedHandler : HttpHandler) = 
    fun(context: HttpContext) -> 
        async {
            let! data = context.BindJson<LogInRequest>()
            if  authenticate data.UserName data.Password
                then let user = getUserClaims data.UserName authScheme
                     do! context.Authentication.SignInAsync(authScheme, user) |> Async.AwaitTask 
                     return Some context 
                else return! authFailedHandler context                                                           
        }

open Nikeza.Server.Models
open Nikeza.Server.DataStore

let private followHandler = 
    fun(context: HttpContext) -> 
        async { let! data = context.BindJson<FollowRequest>()
                execute <| Follow data
                return Some context
        } 

let private unsubscribeHandler = 
    fun(context: HttpContext) -> 
        async { let! data = context.BindJson<UnsubscribeRequest>()
                execute <| Unsubscribe data
                return Some context                  
        } 

let private featureLinkHandler = 
    fun(context: HttpContext) -> 
        async { let! data = context.BindJson<FeatureLinkRequest>()
                execute <| FeatureLink data
                return Some context
        }

let private updateProfileHandler = 
    fun(context: HttpContext) -> 
        async { let! data = context.BindJson<ProfileRequest>()
                execute <| UpdateProfile data
                return Some context
        }

let private addSourceHandler = 
    fun(context: HttpContext) -> 
        async { let! data = context.BindJson<AddSourceRequest>()
                execute <| AddSource data
                return Some context
        }

let private removeSourceHandler = 
    fun(context: HttpContext) -> 
        async { let! data = context.BindJson<RemoveSourceRequest>()
                execute <| RemoveSource data
                return Some context
        }

let private setCode (handler:HttpHandler)= 
    fun(context: HttpContext) ->     
        let response =
             if context.Response.Body.ToString() = ""
                then setStatusCode 401
                else handler
        response context

open Nikeza.Server.YouTube
open Nikeza.Server.YouTube.Authentication
let private fetchYoutube (apiKey, channelId) (context : HttpContext) = 
    async { let  youtube = youTubeService apiKey
            let! videos =  uploadList youtube <| ChannelId channelId
            return! json videos context
    }

open Nikeza.Server.Wordpress
let private fetchWordpress (feedUrl) (context : HttpContext) =
    async { let! response = jsonRssFeed feedUrl
            return! json response context
    }

let private fetchPlatforms (providerId) (context : HttpContext) =
    async { let response = getPlatforms()
            return! json response context
    }

let private fetchLinks (providerId) (context : HttpContext) =
    async { let response = getLinks providerId
            return! json response context
    }
let private fetchFollowers (providerId) (context : HttpContext) =
    async { let response = getFollowers providerId
            return! json response context
    }
let private fetchSubscriptions (providerId) (context : HttpContext) =
    async {
        let response = getSubscriptions providerId
        return! json response context
    }

let private fetchProviders () (context : HttpContext) =
    async { let response = getProviders()
            return! json response context
    }

let private fetchSources (providerId) (context : HttpContext) =
    async { let response = getSources providerId
            return! json response context
    }

let private fetchContentTypeToId (contentType) (context : HttpContext) =
    async { let response = contentTypeToId contentType
            return! json response context
    }

let webApp : HttpContext -> HttpHandlerResult = 

    choose [
        GET >=>
            choose [
                //route "/" >=> htmlFile "/hostingstart.html"
                route "/" >=> htmlFile "/home.html"
                routef "/platforms/%s/%s"    fetchPlatforms
                routef "/youtube/%s/%s"      fetchYoutube
                routef "/wordpress/%s"       fetchWordpress
                routef "/links/%s"           fetchLinks
                routef "/followers/%s"       fetchFollowers
                routef "/subscriptions/%s"   fetchSubscriptions
                routef "/providers"          fetchProviders
                routef "/sources/%s"         fetchSources
                routef "/contenttypetoid/%s" fetchContentTypeToId
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
                route "/addsource"     >=> addSourceHandler
                route "/removesource"  >=> removeSourceHandler
            ]
            
        setStatusCode 404 >=> text "Not Found" ]