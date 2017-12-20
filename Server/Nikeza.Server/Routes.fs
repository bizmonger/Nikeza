module Nikeza.Server.Routes

open System
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Authentication
open Giraffe.HttpContextExtensions
open Giraffe.HttpHandlers
open Giraffe.Tasks
open Store
open Model
open Platforms
open Authentication
open Nikeza.Common

[<Literal>]
let AuthScheme = "Cookie"

open Nikeza.Server.DatabaseCommand.Commands
open System.Threading
open Microsoft.AspNetCore.Authentication.Cookies
open System.Security.Claims
open Nikeza.Common

//-----------------------------------------------------------------------
// DEPLOYMENT
//-----------------------------------------------------------------------
// dotnet publish -c Release -o "C:\Nikeza\deployment" --runtime win8-x64
//-----------------------------------------------------------------------

let private registrationHandler: HttpHandler = 
    fun next ctx -> 
        task {
            let! data = ctx.BindJsonAsync<RegistrationRequest>()
            match Registration.register { data with Email= data.Email.ToLower() } with
            | Success profile -> return! json profile next ctx
            | Failure         -> return! (setStatusCode 400 >=> json "registration failed") next ctx
        }


let authScheme = CookieAuthenticationDefaults.AuthenticationScheme 

let loginHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        
        Tasks.Task.Run(fun _ -> StackOverflow.CachedTags.Instance() |> ignore) |> ignore

        task {
            let! data = ctx.BindJsonAsync<LogInRequest>()
            let  email = data.Email.ToLower()

            if   authenticate email data.Password
                 then match login email with
                      | Some provider -> 
                          let claims = [ Claim(ClaimTypes.Name, email) ]
                          let identity = ClaimsIdentity(claims, authScheme)
                          let user     = ClaimsPrincipal(identity)
                          
                          do!     ctx.SignInAsync(authScheme, user)
                          return! json provider next ctx

                      | None -> return! (setStatusCode 400 >=> json "Invalid login") next ctx
                 else return! (setStatusCode 400 >=> json "Invalid login") next ctx
        }

let private fetchProvider providerId: HttpHandler =
    fun next ctx ->
        Tasks.Task.Run(fun _ -> StackOverflow.CachedTags.Instance() |> ignore) |> ignore
        getProvider providerId
         |> function
           | Some p -> ctx.WriteJsonAsync p
           | None   -> (setStatusCode 400 >=> json "provider not found") next ctx

let private followHandler: HttpHandler = 
    fun next ctx ->
        task { let! data = ctx.BindJsonAsync<FollowRequest>()

               let alreadyFollowing = 
                   data.ProfileId 
                    |> getFollowers 
                    |> List.exists(fun f -> f.Profile.Id = data.SubscriberId)

               let getResult() =
                   match (getProvider data.SubscriberId, getProvider data.ProfileId) with
                   | (Some user, Some provider) -> json { User= user; Provider= provider }
                   | (Some _, None)             -> (setStatusCode 400 >=> json "provider not found")
                   | (None, Some _)             -> (setStatusCode 400 >=> json "user not found")    
                   | (None, None)               -> (setStatusCode 400 >=> json "user and provider not found")

               if not alreadyFollowing
                   then Follow data |> Command.execute |> ignore
                        return! getResult() next ctx
                   else return! getResult() next ctx
             } 

let private unsubscribeHandler: HttpHandler = 
    fun next ctx -> 
        task { let! data = ctx.BindJsonAsync<UnsubscribeRequest>()

               Unsubscribe data |> Command.execute |> ignore
               
               match (getProvider data.SubscriberId, getProvider data.ProfileId) with
               | (Some user, Some provider) -> return! json { User= user; Provider= provider } next ctx
               | (Some _, None)             -> return! (setStatusCode 400 >=> json "provider not found") next ctx
               | (None, Some _)             -> return! (setStatusCode 400 >=> json "user not found")     next ctx
               | (None, None)               -> return! (setStatusCode 400 >=> json "user and provider not found") next ctx
        } 

let private featureLinkHandler: HttpHandler = 
    fun next ctx -> 
        task { 
            let! data = ctx.BindJsonAsync<FeatureLinkRequest>()
            FeatureLink data |> Command.execute |> ignore
            return! json data.LinkId next ctx
        }

let private featuredTopicsHandler: HttpHandler = 
    fun next ctx -> 
        task { 
            let! data = ctx.BindJsonAsync<FeaturedTopicsRequest>()
            UpdateTopics data |> Command.execute |> ignore
            return! fetchProvider data.ProfileId next ctx
        }

let private updateProfileHandler: HttpHandler = 
    fun next ctx -> 
        task { 
            let! data = ctx.BindJsonAsync<ProfileRequest>()
            UpdateProfile data |> Command.execute |> ignore
            return! json data next ctx
        }

let private updateProviderHandler: HttpHandler = 
    fun next ctx -> 
        task { 
            let! data = ctx.BindJsonAsync<ProfileAndTopicsRequest>()
            let topicsRequest = { ProfileId= data.Profile.Id
                                  Names=     data.Topics |> List.map (fun t -> t.Name) }
                                  
            UpdateProfile data.Profile  |> Command.execute |> ignore
            UpdateTopics  topicsRequest |> Command.execute |> ignore

            match getProvider data.Profile.Id with
            | Some provider -> return! json provider next ctx
            | None ->          return! (setStatusCode 400 >=> json "provider not found") next ctx   
        }

let private addSourceHandler: HttpHandler = 
    fun next ctx -> 
        task { 
            let! data =    ctx.BindJsonAsync<DataSourceRequest>()
            let sourceId = AddSource data |> Command.execute
            let links =    data.ProfileId |> Store.linksFrom data.Platform |> List.toSeq
            let source = { data with Id = Int32.Parse(sourceId); Links = links }
            return! json source next ctx
        }

let private removeSourceHandler (sourceId:string): HttpHandler = 
    fun next ctx -> 
        task {
            let id = Int32.Parse(sourceId)
            let (data:RemoveDataSourceRequest) = { Id= id }
            RemoveSource data |> Command.execute |> ignore
            return! json sourceId next ctx
        }

let private addLinkHandler: HttpHandler = 
    fun next ctx -> 
        task { 
            let! data = ctx.BindJsonAsync<Link>()
            let linkId = AddLink { data with Description = ""; Timestamp= DateTime.Now } |> Command.execute
            return! json { data with Id = Int32.Parse(linkId) } next ctx
        }

let private removeLinkHandler: HttpHandler = 
    fun _ ctx -> 
        task { 
            let! data = ctx.BindJsonAsync<RemoveLinkRequest>()
            RemoveLink data |> Command.execute |> ignore
            return Some ctx
        }

let private updateThumbnailHandler: HttpHandler = 
    fun _ ctx -> 
        task { 
            let! data = ctx.BindJsonAsync<UpdateThumbnailRequest>()
            UpdateThumbnail data |> Command.execute |> ignore
            return Some ctx
        }

let private fetchBootstrap x: HttpHandler =

    Tasks.Task.Run(fun _ -> StackOverflow.CachedTags.Instance() |> ignore) |> ignore

    let providers = getProviders()
    json { Providers= providers; Platforms=getPlatforms() }

let private syncSources x: HttpHandler =

    getAllSources() |> List.iter (fun s -> s |> syncDataSource |> ignore)
    json []

let private fetchProviders x: HttpHandler =
    json <| getProviders()

let private fetchLinks providerId: HttpHandler =

    Tasks.Task.Run(fun _ -> StackOverflow.CachedTags.Instance() |> ignore) |> ignore

    providerId 
     |> getProvider
     |> function
        | Some p -> json <| { p with Portfolio= providerId 
                                                 |> getLinks 
                                                 |> toPortfolio }
        | None   -> json []

let private fetchSuggestedTopics (text) =
    json <| Suggestions.getSuggestions text

let private fetchRecent (subscriberId) =
    json <| getProvidersWithRecent subscriberId
    
let private fetchFollowers (providerId) =
    json <| getFollowers providerId
    
let private fetchSubscriptions (providerId) =
    json <| getSubscriptions providerId

let private fetchSources (providerId) =
    json <| getSources providerId

let private fetchThumbnail (platform:string , accessId:string) =

    let thumbnail() =
        platform.ToLower() 
         |> platformFromString 
         |> Platforms.getThumbnail accessId
                                 
    json { ImageUrl= thumbnail(); Platform= platform }

let private OnLandingPage: HttpHandler = 
    htmlFile "index.html"
    //fun next ctx ->
    //    task {
    //        let isAuthenticated = ctx.User.Identity.IsAuthenticated

    //        if isAuthenticated
    //           then let email = ctx.User.Identity.Name
    //                match login email with
    //                | Some provider -> return! json provider next ctx
    //                | None          -> return htmlFile "index.html" // Compile Error
    //           else return htmlFile "index.html"                    // Compile Error
        //}
    
let webApp: HttpHandler = 
    choose [
        GET >=>
            choose [
                route "/"                   >=> OnLandingPage
                route  "/options"           >=> setHttpHeader "Allow" "GET, OPTIONS, POST" // CORS support
                routef "/syncsources/%s"        syncSources
                routef "/bootstrap/%s"          fetchBootstrap
                routef "/providers/%s"          fetchProviders
                routef "/links/%s"              fetchLinks
                routef "/suggestedtopics/%s"    fetchSuggestedTopics
                routef "/recent/%s"             fetchRecent
                routef "/followers/%s"          fetchFollowers
                routef "/subscriptions/%s"      fetchSubscriptions
                routef "/sources/%s"            fetchSources
                routef "/thumbnail/%s/%s"       fetchThumbnail
                routef "/provider/%s"           fetchProvider
                routef "/removesource/%s"       removeSourceHandler
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
                route "/updateprovider"  >=> updateProviderHandler
                route "/addsource"       >=> addSourceHandler
                route "/addlink"         >=> addLinkHandler
                route "/removelink"      >=> removeLinkHandler
                route "/updatethumbnail" >=> updateThumbnailHandler
                route "/featuredtopics"  >=> featuredTopicsHandler
            ]
            
        setStatusCode 404 >=> text "Not Found" ]