module Nikeza.Server.WordPress

    open Newtonsoft.Json
    open Nikeza.Server.Model
    open Nikeza.Server.Http
    open System.Collections.Generic

    [<Literal>]
    let private APIBaseAddress = "https://public-api.wordpress.com/"

    [<Literal>]
    let private ArticlesUrl =    "rest/v1/sites/{0}/posts?number=100&page={1}"

    [<Literal>]
    let private ThumbnailUrl =   "rest/v1/sites/bizmonger.wordpress.com/posts?number=1&page=1"

    type Tag = { ID: string; name: string }

    type Post = { 
        title: string 
        URL:   string 
        Tags:  IDictionary<string, Tag>
    }

    type Response = { found: int; posts: Post list }

    let private toLink profileId (post:Post) =
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

    let getThumbnail accessId =
 
        let response = sendRequest APIBaseAddress ThumbnailUrl accessId <| string 1

        if response.IsSuccessStatusCode
           then let json = response.Content.ReadAsStringAsync() |> Async.AwaitTask 
                                                                |> Async.RunSynchronously

                let line = json.Split("\"avatar_URL\"")
                                .[1]
                                .Split(',')
                                .[0]
                                .Replace("\\", "")      
                let startIndex = line.IndexOf("https:")
                let url = line.Substring(startIndex, line.Length - 3)
                url
           else ""

    let rec wordpressLinks (user:User) (pageNumber:int) existingLinks =

        let response = sendRequest APIBaseAddress ArticlesUrl user.AccessId <| string pageNumber

        if  response.IsSuccessStatusCode
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