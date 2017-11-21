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
open Registration
open YouTube
open StackOverflow.Suggestions
open Order

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

let private removeSourceHandler (sourceId:string): HttpHandler = 
    fun next ctx -> 
        task {
            let id = Int32.Parse(sourceId)
            let (data:RemoveDataSourceRequest) = { Id= id }
            RemoveSource data |> execute |> ignore
            return! json sourceId next ctx
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
    json { Providers= getProviders(); Platforms=getPlatforms() }

let private fetchLinks providerId: HttpHandler =
    json (getLinks providerId)

let private fetchSuggestedTopics (text) =
    json (getSuggestions text)

let private fetchRecent (subscriberId) =
    json (getRecent subscriberId)
    
let private fetchFollowers (providerId) =
    json (getFollowers providerId)
    
let private fetchSubscriptions (providerId) =
    json (getSubscriptions providerId)

let private fetchSources (providerId) =
    json (getSources providerId)

let private fetchThumbnail (platform:string , accessId:string) =

    let thumbnail = 
        platform.ToLower() 
        |> platformFromString 
        |> getThumbnail accessId
                                       
    json { ImageUrl= thumbnail; Platform= platform }
    
let private fetchContentTypeToId (contentType) =
    json (contentTypeToId contentType)
         
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
                routef "/removesource/%s"        removeSourceHandler
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
                route "/addlink"         >=> addLinkHandler
                route "/removelink"      >=> removeLinkHandler
                route "/updatethumbnail" >=> updateThumbnailHandler
            ]
            
        setStatusCode 404 >=> text "Not Found" ]