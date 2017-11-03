module Nikeza.Server.Medium

    open System
    open Model
    open Http
    open Newtonsoft.Json

    [<Literal>]
    let BaseAddress = "https://medium.com/"

    let private parseValue (line:string) =
        if line.Contains(":")
            then Some <| line.Split(':')
                             .[1]
                             .Replace("\"", "")
                             .Trim()
                             .TrimEnd(',')
            else None
                     
    let private getPostBlock (text:string) =
        let postsIndex =  text.IndexOf("\"Post\": {") + 11
        let partial =     text.Substring(postsIndex, text.Length - postsIndex)
        let postEndIndex= partial.IndexOf("},")
        let postBlock=    partial.Substring(0, postEndIndex)
        postBlock

    let private createLink (postBlock:string) (user:User) =

        let getTagsBlock (postBlock:string) =
            let startIndex = postBlock.IndexOf("\"tags\": [")
            let block = postBlock.Substring(startIndex, postBlock.Length - startIndex)
            let endIndex = block.IndexOf("],")
            let tagsBlock = block.Substring(0, endIndex)
            tagsBlock

        let getTagBlock (tagsBlock:string) =
            let startIndex = tagsBlock.IndexOf('{')

            if startIndex >= 0
                then let endIndex =   tagsBlock.IndexOf('}')
                     let tagBlock =   tagsBlock.Substring(startIndex, endIndex)
                     Some tagBlock
                else None

        let rec getTags (postBlock:string) (tags: (string option) list) =

            let rec getTag (postBlock:string) : string option =
                let tagBlock =   postBlock |> getTagsBlock |> getTagBlock

                match tagBlock with
                    | None       -> None
                    | Some block ->
                        if block.Contains("\"slug\":")
                            then let tagParts = block.Split('\n')
                                 let tag =      parseValue(tagParts.[1])
                                 tag
                            else let truncated = postBlock.Replace(block, "")
                                 getTag truncated
            
            let tag =      getTag postBlock
            let tagBlock = postBlock |> getTagsBlock |> getTagBlock

            match tagBlock with
            | None       -> tags
            | Some block ->
                if block.Contains("\"slug\":")
                    then let truncated = postBlock.Replace(block, "")
                         tag::tags |> getTags truncated
                    else let nextBlock = postBlock |> getTagsBlock |> getTagBlock
                         match nextBlock with
                         | Some t -> 
                            let truncated = postBlock.Replace(t, "")
                            tag::tags |> getTags truncated
                         | None   -> tag::tags

        let  postParts = postBlock.Split("\n")

        let topics = [] |> getTags postBlock
                        |> List.choose(fun tag -> match tag with
                                                  | Some t -> Some { Id= -1; Name=t; }
                                                  | None   -> None
                                      )
        let id = parseValue(postParts.[1]) |> function | Some title -> "{0}" + title | None -> ""

        { Id= -1
          ProfileId=    user.ProfileId
          Title=        parseValue(postParts.[5]) |> function | Some title -> title | None -> ""
          Description=  ""
          Url=          String.Format("{0}{1}/{2}", BaseAddress, "@" + user.AccessId, id)
          Topics=       topics |>List.toSeq |> Seq.distinct |> Seq.toList
          ContentType= "Article"
          IsFeatured=   false
        }

    let private remainingText nextTagIndex (text:string) =
        let newIndex =       if nextTagIndex >= 300 then nextTagIndex - 300 else nextTagIndex
        let nextPostTemp =   text.Substring(newIndex, text.Length - newIndex)
        let newEndIndex =    nextPostTemp.IndexOf(": {")
        let nextPost =       nextPostTemp.Substring(newEndIndex, nextPostTemp.Length - newEndIndex)
        nextPost

    let private getNextPost (text:string) (postBlock:string) =
        let tagsIndex =      text.IndexOf("\"tags\": [")
        let tagsBlock1 =     text.Substring(tagsIndex, text.Length - tagsIndex)
        let tagsEndIndex=    tagsBlock1.IndexOf("],")
        let removeTagBlock=  tagsBlock1.Substring(0, tagsEndIndex)
        let truncatedText1 = text.Replace(removeTagBlock, "")
        let truncatedText2 = truncatedText1.Replace(postBlock, "")
        let nextPostIndex =  truncatedText2.IndexOf("\"homeCollectionId\":")
        let nextPost =       remainingText nextPostIndex truncatedText2
        nextPost

    let rec private linksFrom (partial:string) (user:User) (originalContent:string) links =

        let identifier =   "\"homeCollectionId\":"
        let nextTagIndex =  partial.IndexOf(identifier)

        if nextTagIndex >= 0
            then let nextPost = remainingText nextTagIndex partial

                 if nextPost.Contains(identifier)
                    then let endIndex =        partial.IndexOf("\"homeCollectionId\":", partial.IndexOf("\"homeCollectionId\":") + 1)
                         let entirePostBlock = partial.Substring(0, endIndex - 300)
                         let link =            user |> createLink entirePostBlock
                         let postBlock =       getPostBlock originalContent
                         let content =         getNextPost nextPost postBlock
                         
                         [link] |> List.append links 
                                |> linksFrom content user originalContent

                    else let link = user |> createLink partial
                         List.append links [link]
            else links


    let formatJson json =
        let parsedJson = JsonConvert.DeserializeObject(json);
        JsonConvert.SerializeObject(parsedJson, Formatting.Indented);

    let mediumLinks (user:User) =

        let url =      String.Format("@{0}/latest?format=json", user.AccessId)
        let client =   httpClient BaseAddress
        let response = client.GetAsync(url) |> Async.AwaitTask 
                                            |> Async.RunSynchronously
        if response.IsSuccessStatusCode
            then let rawText = response.Content.ReadAsStringAsync() |> Async.AwaitTask 
                                                                    |> Async.RunSynchronously
                 let text = rawText.Replace("])}while(1);</x>", "")
                 let json = text |> formatJson
                 let postsIndex = json.IndexOf("\"Post\": {") + 11
                 let postsBlock = json.Substring(postsIndex, json.Length - postsIndex)
                 [] |> linksFrom postsBlock user json
                    |> List.toSeq
            else Seq.empty