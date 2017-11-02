module Nikeza.Server.Wordpress

    let getThumbnail accessId apiKey = ""

    open System
    open System.IO
    open System.Net.Http
    open Newtonsoft.Json
    open Nikeza.Server.Model
    open Nikeza.Server.Http
    open Nikeza.Server.Literals
    open Newtonsoft.Json.Linq
    open System.Collections.Generic

    [<Literal>]
    let APIBaseAddress = "https://public-api.wordpress.com/"

    [<Literal>]
    let ArticlesUrl =     "rest/v1/sites/{0}/posts?number=100&page={1}"

    type Tag = { ID: string; name: string }

    type Post = { 
        title: string 
        URL:   string 
        Tags:  IDictionary<string, Tag>
    }

    type Response = { found: int; posts: Post list }

    let toLink profileId (post:Post) =
        let stringToTag t = { 
            Id= -1
            Name= t.ToString()                   
                   .Remove(0,1)
                   .Split(",")
                   .[0] 
            }

        { Id= -1
          ProfileId= profileId
          Title= post.title
          Description= ""
          Url= post.URL
          Topics= List.ofSeq post.Tags |> List.map stringToTag
          ContentType="Articles"
          IsFeatured= false
        }

    let rec wordpressLinks (user:User) (pageNumber:int) existingLinks =

        let foo (client:HttpClient) =
            let url =        String.Format(ArticlesUrl, user.AccessId, pageNumber |> string)
            let response =   client.GetAsync(url) |> Async.AwaitTask 
                                                  |> Async.RunSynchronously
            if response.IsSuccessStatusCode
            then let json =     response.Content.ReadAsStringAsync() |> Async.AwaitTask 
                                                                     |> Async.RunSynchronously
                 let settings = JsonSerializerSettings()
                 settings.MissingMemberHandling <- MissingMemberHandling.Ignore
 
                 let result =      JsonConvert.DeserializeObject<Response>(json, settings)
                 let lastPage =    (result.found / 100) + 1
                 let canContinue = lastPage >= pageNumber
                 if canContinue
                     then result.posts |> Seq.map (fun post -> toLink user.ProfileId post)
                                       |> Seq.append <| existingLinks
                                       |> wordpressLinks user (pageNumber + 1)
                     else existingLinks
            else seq []
        
        use client = httpClient APIBaseAddress
        foo client