module Nikeza.Server.Routes

open System
open Microsoft.AspNetCore.Http
open Giraffe.HttpContextExtensions
open Giraffe.HttpHandlers
open Store
open Model
open Platforms
open Authentication
open Giraffe.Tasks
open Command
open StackOverflow.Suggestions

[<Literal>]
let AuthScheme = "Cookie"

let private registrationHandler: HttpHandler = 
    fun next ctx -> 
        task {
            let! data = ctx.BindJson<RegistrationRequest>()
            match register data with
            | Success profile -> return! json profile next ctx
            | Failure         -> return! (setStatusCode 400 >=> json "registration failed") next ctx
        }

let private loginHandler: HttpHandler = 
    fun next ctx -> 
        task {
            let! data = ctx.BindJson<LogInRequest>()
            if  authenticate data.Email data.Password
                then match login data.Email with
                     | Some provider -> return! json provider next ctx
                     | None          -> return! (setStatusCode 400 >=> json "Invalid login") next ctx
                else return! (setStatusCode 400 >=> json "Invalid login") next ctx                                                       
        }

let private fetchProvider providerId: HttpHandler =
    fun next ctx ->
        getProvider providerId
        |> function
          | Some p -> ctx.WriteJson p
          | None   -> (setStatusCode 400 >=> json "provider not found") next ctx

let private followHandler: HttpHandler = 
    fun next ctx -> 
        task { 
            let! data = ctx.BindJson<FollowRequest>()
            Follow data |> execute |> ignore
            return! fetchProvider data.ProfileId next ctx 
        } 

let private unsubscribeHandler: HttpHandler = 
    fun next ctx -> 
        task { 
            let! data = ctx.BindJson<UnsubscribeRequest>()
            Unsubscribe data |> execute |> ignore
            return! fetchProvider data.ProfileId next ctx          
        } 

let private featureLinkHandler: HttpHandler = 
    fun next ctx -> 
        task { 
            let! data = ctx.BindJson<FeatureLinkRequest>()
            FeatureLink data |> execute |> ignore
            return! json data.LinkId next ctx
        }

let private updateProfileHandler: HttpHandler = 
    fun next ctx -> 
        task { 
            let! data = ctx.BindJson<ProfileRequest>()
            UpdateProfile data |> execute |> ignore
            return! json data next ctx
        }

let private addSourceHandler: HttpHandler = 
    fun next ctx -> 
        task { 
            let! data = ctx.BindJson<DataSourceRequest>()
            let sourceId = AddSource data |> execute
            let links =    data.ProfileId |> Store.linksFrom data.Platform |> List.toSeq
            let source = { data with Id = Int32.Parse(sourceId); Links = links }
            return! json source next ctx
        }

let private removeSourceHandler: HttpHandler = 
    fun next ctx -> 
        task { 
            let! data = ctx.BindJson<RemoveDataSourceRequest>()
            RemoveSource data |> execute |> ignore
            return! json data next ctx
        }

let private addLinkHandler: HttpHandler = 
    fun next ctx -> 
        task { 
            let! data = ctx.BindJson<Link>()
            let linkId = AddLink { data with Description = "" } |> execute
            return! json { data with Id = Int32.Parse(linkId) } next ctx
        }

let private removeLinkHandler: HttpHandler = 
    fun _ ctx -> 
        task { 
            let! data = ctx.BindJson<RemoveLinkRequest>()
            RemoveLink data |> execute |> ignore
            return Some ctx
        }

let private updateThumbnailHandler: HttpHandler = 
    fun _ ctx -> 
        task { 
            let! data = ctx.BindJson<UpdateThumbnailRequest>()
            UpdateThumbnail data |> execute |> ignore
            return Some ctx
        }

let private fetchBootstrap: HttpHandler =
    StackOverflow.CachedTags.Instance() |> ignore
    let  dependencies = { Providers= getProviders(); Platforms=getPlatforms() }
    json dependencies

type Resposne =  HttpFunc -> HttpContext -> HttpFuncResult
let private fetchLinks providerId: HttpHandler =
    let response = getLinks providerId
    json response

let private fetchSuggestedTopics (text) =
    let  suggestions = getSuggestions text
    json suggestions

let private fetchRecent (subscriberId) =
    let  response = getRecent subscriberId
    json response
    
let private fetchFollowers (providerId) =
    let  response = getFollowers providerId
    json response
    
let private fetchSubscriptions (providerId) =
    let response = getSubscriptions providerId
    json response

let private fetchSources (providerId) =
    let  response = getSources providerId
    json response

let private fetchThumbnail (platform:string , accessId:string) =

    let thumbnail = 
        platform.ToLower() 
        |> platformFromString 
        |> getThumbnail accessId
                                       
    json { ImageUrl= thumbnail; Platform= platform }
    
let private fetchContentTypeToId (contentType) =
    let response = contentTypeToId contentType
    json response
         
let webApp: HttpHandler = 
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
                routef "/provider/%s"            fetchProvider
            ]
        POST >=> 
            choose [
                route "/register"        >=> registrationHandler
                route "/login"           >=> loginHandler
                route "/logout"          >=> signOff AuthScheme >=> text "logged out"
                route "/follow"          >=> followHandler
                route "/unsubscribe"     >=> unsubscribeHandler
                route "/featurelink"     >=> featureLinkHandler
                route "/updateprofile"   >=> updateProfileHandler
                route "/addsource"       >=> addSourceHandler
                route "/removesource"    >=> removeSourceHandler
                route "/addlink"         >=> addLinkHandler
                route "/removelink"      >=> removeLinkHandler
                route "/updatethumbnail" >=> updateThumbnailHandler
            ]
            
        setStatusCode 404 >=> text "Not Found" ]