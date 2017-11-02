module Nikeza.Server.Routes

open System
open Microsoft.AspNetCore.Http
open Giraffe.HttpContextExtensions
open Giraffe.HttpHandlers
open Nikeza.Server.Store
open Nikeza.Server.Model
open Nikeza.Server.Platforms
open Nikeza.Server.Authentication

let authScheme = "Cookie"

let private registrationHandler = 
    fun(context: HttpContext) -> 
        async {
            let! data = context.BindJson<RegistrationRequest>()
            match register data with
            | Success profile -> return! json profile context
            | Failure         -> return! (setStatusCode 400 >=> json "registration failed") context
        }

let private loginHandler = 
    fun(context: HttpContext) -> 
        async {
            let! data = context.BindJson<LogInRequest>()
            if  authenticate data.Email data.Password
                then match loginProvider data.Email with
                     | Some provider -> return! json provider context 
                     | None          -> return! (setStatusCode 400 >=> json "Invalid login") context
                else return! (setStatusCode 400 >=> json "Invalid login") context                                                        
        }

open Nikeza.Server.Command

let private followHandler = 
    fun(context: HttpContext) -> 
        async { let! data = context.BindJson<FollowRequest>()
                ignore (execute <| Follow data)
                return Some context
        } 

let private unsubscribeHandler = 
    fun(context: HttpContext) -> 
        async { let! data = context.BindJson<UnsubscribeRequest>()
                ignore (execute <| Unsubscribe data)
                return Some context                  
        } 

let private featureLinkHandler = 
    fun(context: HttpContext) -> 
        async { let! data = context.BindJson<FeatureLinkRequest>()
                let link = FeatureLink data |> execute
                return! json link context
        }

let private updateProfileHandler = 
    fun(context: HttpContext) -> 
        async { let! data = context.BindJson<ProfileRequest>()
                UpdateProfile data |> execute |> ignore
                return! json data context
        }

let private addSourceHandler = 
    fun(context: HttpContext) -> 
        async { let! data = context.BindJson<DataSourceRequest>()
                let sourceId = AddSource data |> execute
                let links =    data.ProfileId |> linksFrom data.Platform |> List.toSeq
                let source = { data with Id = Int32.Parse(sourceId); Links = links }
                return! json source context
        }

let private removeSourceHandler = 
    fun(context: HttpContext) -> 
        async { let! data = context.BindJson<RemoveDataSourceRequest>()
                RemoveSource data |> execute |> ignore
                return! json data context
        }

let private addLinkHandler = 
    fun(context: HttpContext) -> 
        async { let! data = context.BindJson<Link>()
                let linkId = AddLink { data with Description = "" } |> execute
                return! json { data with Id = Int32.Parse(linkId) } context
        }

let private removeLinkHandler = 
    fun(context: HttpContext) -> 
        async { let! data = context.BindJson<RemoveLinkRequest>()
                ignore (execute <| RemoveLink data)
                return Some context
        }

open Nikeza.Server.Wordpress
open Nikeza.Server.StackOverflow
open Nikeza.Server.StackOverflow.Suggestions

let private fetchBootstrap =
    CachedTags.Instance() |> ignore
    let  dependencies = { Providers= getProviders(); Platforms=getPlatforms() }
    json dependencies

let private fetchLinks (providerId) (context : HttpContext) =
    let response = getLinks providerId
    json response context

let private fetchSuggestedTopics (text) (context : HttpContext) =
    let  suggestions = getSuggestions text
    json suggestions context

let private fetchRecent (subscriberId) (context : HttpContext) =
    let  response = getRecent subscriberId
    json response context
    
let private fetchFollowers (providerId) (context : HttpContext) =
    let  response = getFollowers providerId
    json response context
    
let private fetchSubscriptions (providerId) (context : HttpContext) =
        let response = getSubscriptions providerId
        json response context

let private fetchSources (providerId) (context : HttpContext) =
    let  response = getSources providerId
    json response context

let private fetchThumbnail (platform:string, accessId:string) (context : HttpContext) =

    let platform =  platform.ToLower() |> PlatformFromString
    let thumbnail = Platforms.getThumbnail platform accessId

    json thumbnail context
    
let private fetchContentTypeToId (contentType) (context : HttpContext) =
    let response = contentTypeToId contentType
    json response context
         
let webApp : HttpContext -> HttpHandlerResult = 

    choose [
        GET >=>
            choose [
                //route "/" >=> htmlFile "/hostingstart.html"
                route  "/"                  >=>  htmlFile "/home.html"
                route  "/options"           >=>  setHttpHeader "Allow" "GET, OPTIONS, POST" // CORS support
                route  "/bootstrap"         >=>  fetchBootstrap
                routef "/links/%s"               fetchLinks
                routef "/suggestedtopics/%s"     fetchSuggestedTopics
                routef "/recent/%s"              fetchRecent
                routef "/followers/%s"           fetchFollowers
                routef "/subscriptions/%s"       fetchSubscriptions
                routef "/sources/%s"             fetchSources
                routef "/thumbnail/%s/%s"        fetchThumbnail
                routef "/contenttypetoid/%s"     fetchContentTypeToId
            ]
        POST >=> 
            choose [
                route "/register"      >=> registrationHandler
                route "/login"         >=> loginHandler
                route "/logout"        >=> signOff authScheme >=> text "logged out"
                route "/follow"        >=> followHandler
                route "/unsubscribe"   >=> unsubscribeHandler
                route "/featurelink"   >=> featureLinkHandler
                route "/updateprofile" >=> updateProfileHandler
                route "/addsource"     >=> addSourceHandler
                route "/removesource"  >=> removeSourceHandler
                route "/addlink"       >=> addLinkHandler
                route "/removelink"    >=> removeLinkHandler
            ]
            
        setStatusCode 404 >=> text "Not Found" ]