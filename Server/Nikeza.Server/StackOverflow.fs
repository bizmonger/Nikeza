namespace Nikeza.Server

module StackOverflow =

    open System
    open System.IO
    open Newtonsoft.Json
    open Utils
    open Model
    open Http
    open Literals
    open SuggestionFinder

    [<Literal>]
    let private TagsUrl =    "2.2/tags?page={0}&order=desc&sort=popular&site=stackoverflow&filter=!-.G.68grSaJm"

    [<Literal>]
    let private ThumbnailUrl = "2.2/users/{0}?order=desc&sort=reputation&site=stackoverflow&filter=!40DELPbCy)uCaG7xi&key={1}"

    [<Literal>]
    let private AnswersUrl = "2.2/users/{0}/answers?page={1}&pagesize=100&order=desc&sort=activity&site=stackoverflow&filter=!Fcazzsr2b3M)LbUjGAu-Fs0Wf8&key={2}"

    [<Literal>]
    let private APIBaseAddress = "https://api.stackexchange.com/"

    [<Literal>]
    let private SiteBaseAddress = "https://stackoverflow.com/"

    type Answer = {
        link:          string
        title:         string
        name:          string
        tags:          string list
        isAccepted:    bool
        creation_date: int
        answer_id:     int
        question_id:   int
    }

    type AnswersResponse = { 
        items:           Answer list
        has_more:        bool
        quota_max:       int
        quota_remaining: int
    }

    type Thumbnail =         { profile_image: string }
    type ThumbnailResponse = { items: Thumbnail list }

    let private toLink profileId item =
        { Id=          -1
          ProfileId=   profileId
          Title=       item.title |> replaceHtmlCodes
          Description= ""
          Url=         item.link
          Topics=      item.tags |> List.map (fun t -> { Id= -1; Name= t; IsFeatured= false })
          ContentType= "Answers"
          IsFeatured=  false
          Timestamp=   DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds((float item.creation_date))
        }

    let getThumbnail accessId key =

        let url =      String.Format(ThumbnailUrl, accessId, key)
        let response = sendRequest APIBaseAddress url accessId key

        if response.IsSuccessStatusCode
           then let json =   response.Content.ReadAsStringAsync() |> toResult
                let result = JsonConvert.DeserializeObject<ThumbnailResponse>(json)
                result.items |> function
                | h::_ -> h.profile_image
                | []   -> ThumbnailUrl

           else ThumbnailUrl

    let rec private getLinks platformUser (pageNumber:int) existingLinks =

        let user =     platformUser.User
        let url =      String.Format(AnswersUrl, user.AccessId, pageNumber, platformUser.APIKey)
        let response = sendRequest APIBaseAddress url user.AccessId platformUser.APIKey

        if response.IsSuccessStatusCode
           then let json =  response.Content.ReadAsStringAsync() |> toResult
                let links =
                    JsonConvert.DeserializeObject<AnswersResponse>(json).items
                     |> Seq.toList
                     |> List.map (fun item -> toLink user.ProfileId item)
                     |> List.rev

                if links |> List.isEmpty
                   then existingLinks
                   else links 
                         |> List.append existingLinks 
                         |> getLinks platformUser (pageNumber + 1)
           else []

    let stackoverflowLinks platformUser =
        [] |> getLinks platformUser 1
 
    type Tag =          { name : string }
    type TagsResponse = { items: Tag list }

    let private getTags (pageNumber:int) : string list =

        let client = httpClient APIBaseAddress

        try let url =        String.Format(TagsUrl, pageNumber |> string)
            let urlWithKey = sprintf "%s&key=%s" url (File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), KeyFile_StackOverflow)))
            let response =   client.GetAsync(urlWithKey) |> toResult
            if response.IsSuccessStatusCode
               then let json = response.Content.ReadAsStringAsync() |> toResult
                    JsonConvert.DeserializeObject<TagsResponse>(json)
                               .items 
                                |> List.ofSeq 
                                |> List.map (fun i -> i.name)
               else []

        finally  client.Dispose()


    module CachedTags =
        
        let private x = [1..25] |> List.collect getTags
        let Instance() = x


    module Suggestions =

        let suggestionsFromText (title:string) =
            let allTags = CachedTags.Instance() |> Set.ofList

            title.Split(' ')
             |> Array.map(fun w -> w.Replace(":", "")
                                    .Replace(",", "")
                                    .Trim().ToLower())
             |> Set.ofArray
             |> Set.intersect allTags 
             |> Set.toList
            
        let getRelatedTags (tag:string) =

            if tag <> ""
            then let parseTag (text:string) =
                     let index = text.IndexOf("|")
                     if  index > 0
                         then Some <| text.Substring(0,index)
                         else None
                 
                 let client = httpClient SiteBaseAddress
         
                 try let relatedTagsUrl = sprintf "filter/tags?q=%s" tag
                     let response =       client.GetAsync(relatedTagsUrl) |> toResult
                     if response.IsSuccessStatusCode
                     then let result = response.Content.ReadAsStringAsync() |> toResult
                          result.Split('\n') 
                           |> List.ofArray 
                           |> List.filter (fun x -> x <> "")
                           |> List.tryHead
                           |> function 
                              | None -> []
                              | Some formatted ->
                                  let tags = formatted.Split("\\n") 
                                              |> List.ofArray
                                              |> List.choose parseTag
                                              |> List.map   (fun tag -> tag.Replace(@"""", ""))
                                              |> List.filter(fun current -> current <> tag)
                                  tag::tags
                     else []
     
                 finally client.Dispose()
            else []

        let getAllTags (searchItem:string) =
            if searchItem <> "" && searchItem.Length > 1
                then CachedTags.Instance() |> List.map (fun t -> t.ToLower())
                else []