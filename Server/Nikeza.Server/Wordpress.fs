module Nikeza.Server.Wordpress

    let getThumbnail accessId apiKey = ""

    open System
    open System.IO
    open Newtonsoft.Json
    open Nikeza.Server.Model
    open Nikeza.Server.Http
    open Nikeza.Server.Literals
    open Newtonsoft.Json.Linq

    [<Literal>]
    let APIBaseAddress = "https://public-api.wordpress.com/"

    [<Literal>]
    let ArticlesUrl =     "rest/v1/sites/0}/posts"

    type Post = { 
        title: string
        URL:   string
        Tags:  JToken list
    }

    type Response = { posts: Post list }

    let toLink profileId (post:Post) =
        { Id= -1
          ProfileId= profileId
          Title= post.title
          Description= ""
          Url= post.URL
          Topics= post.Tags |> List.map (fun t -> { Id= -1; Name= t.ToString() })
          ContentType="Answers"
          IsFeatured= false
        }

    let wordpressLinks (user:User) =
        let client = httpClient APIBaseAddress

        try let url =        String.Format(ArticlesUrl, user.AccessId)
            let response =   client.GetAsync(url) |> Async.AwaitTask 
                                                  |> Async.RunSynchronously
            if response.IsSuccessStatusCode
            then let json =     response.Content.ReadAsStringAsync() |> Async.AwaitTask 
                                                                     |> Async.RunSynchronously
                 let settings = JsonSerializerSettings()
                 settings.MissingMemberHandling <- MissingMemberHandling.Ignore
 
                 let result = JsonConvert.DeserializeObject<Response>(json, settings);

                 let links =   result.posts |> Seq.map (fun post -> toLink user.ProfileId post)
                 links
            else seq []
            
        finally client.Dispose()
