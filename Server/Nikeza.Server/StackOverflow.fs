namespace Nikeza.Server

module StackOverflow =

    open System
    open System.IO
    open Newtonsoft.Json
    open Model
    open Http
    open Literals

    [<Literal>]
    let private TagsUrl =    "2.2/tags?page={0}&order=desc&sort=popular&site=stackoverflow&filter=!-.G.68grSaJm"

    [<Literal>]
    let private ThumbnailUrl = "2.2/users/{0}?order=desc&sort=reputation&site=stackoverflow&filter=!40DELPbCy)uCaG7xi&key={1}"

    [<Literal>]
    let private AnswersUrl = "2.2/users/{0}/answers?order=desc&sort=activity&site=stackoverflow&filter=!Fcazzsr2b3M)LbUjGAu-Fs0Wf8&key={1}"

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
          Title=       item.title
          Description= ""
          Url=         item.link
          Topics=      item.tags |> List.map (fun t -> { Id= -1; Name= t; IsFeatured= false })
          ContentType= "Answers"
          IsFeatured=  false
        }

    let getThumbnail accessId key =
        let url =      String.Format(ThumbnailUrl, accessId, key)
        let response = sendRequest APIBaseAddress url accessId key

        if response.IsSuccessStatusCode
           then let json =   response.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
                let result = JsonConvert.DeserializeObject<ThumbnailResponse>(json)
                result.items |> function
                | h::_ -> h.profile_image
                | []   -> ThumbnailUrl

           else ThumbnailUrl

    let stackoverflowLinks platformUser =
        let user =     platformUser.User
        let url =      String.Format(AnswersUrl, user.AccessId, platformUser.APIKey)
        let response = sendRequest APIBaseAddress url user.AccessId platformUser.APIKey

        if response.IsSuccessStatusCode
           then let json =   response.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
                JsonConvert.DeserializeObject<AnswersResponse>(json).items
                |> Seq.map (fun item -> toLink user.ProfileId item)
           else seq []

    type Tag =          { name : string }
    type TagsResponse = { items: Tag list }

    let private getTags (pageNumber:int) : string list =

        let client = httpClient APIBaseAddress

        try let url =        String.Format(TagsUrl, pageNumber |> string)
            let urlWithKey = sprintf "%s&key=%s" url (File.ReadAllText(KeyFile_StackOverflow))
            let response =   client.GetAsync(urlWithKey) |> Async.AwaitTask 
                                                         |> Async.RunSynchronously
            if response.IsSuccessStatusCode
               then let json = response.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously
                    JsonConvert.DeserializeObject<TagsResponse>(json).items 
                    |> List.ofSeq |> List.map (fun i -> i.name)
               else []

        finally  client.Dispose()


    module CachedTags =
        
        let private x = [1..25] |> List.collect (fun page -> getTags(page))
        let Instance() = x


    module Suggestions =

        let getRelatedTags (tag:string) =

            if tag <> ""
            then let parseTag (text:string) =
                     let index = text.IndexOf("|")
                     if  index > 0
                         then Some <| text.Substring(0,index)
                         else None
                 
                 let client = httpClient SiteBaseAddress
         
                 try let relatedTagsUrl = sprintf "filter/tags?q=%s" tag
                     let response =       client.GetAsync(relatedTagsUrl) |> Async.AwaitTask 
                                                                          |> Async.RunSynchronously
                     if response.IsSuccessStatusCode
                     then let result = response.Content.ReadAsStringAsync() |> Async.AwaitTask 
                                                                            |> Async.RunSynchronously 
                          result.Split('\n') |> List.ofArray 
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
                
        let getSuggestions (searchItem:string) =
            if searchItem <> ""
            then let tags =         CachedTags.Instance() |> List.map (fun t -> t.ToLower())
                 let filteredTags = tags |> List.filter(fun t -> t.Contains(searchItem.ToLower()))
                 let matchingTags = filteredTags |> List.filter (fun t -> t = searchItem)

                 if matchingTags |> List.isEmpty |> not
                    then getRelatedTags matchingTags.Head
                    else filteredTags
            else []