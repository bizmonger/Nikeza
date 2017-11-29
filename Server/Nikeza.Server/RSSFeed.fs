module Nikeza.Server.RSSFeed

    open System.Xml.Linq
    open System
    open Model
    open Http
    open Asynctify
    open StackOverflow.Suggestions
    
    let rssLinks (user:User) =

        let toLink (item:XElement) =
        
            let (text:string) = item.Element(XName.Get("pubDate")).Value
            let array =    text.Split(' ')
            let day =      array.[1]
            let month =    array.[2] |> monthTextToInteger
            let year =     array.[3]
            let dateText = sprintf "%i/%s/%s" month day year
            let date =     DateTime.Parse(dateText)

            {   Id=          -1
                ProfileId=   user.ProfileId
                Title=       item.Element(XName.Get("title")).Value |> replaceHtmlCodes
                Url=         item.Element(XName.Get("link")).Value
                Description= item.Element(XName.Get("description")).Value
                ContentType= Podcast |> contentTypeToString
                Topics =     suggestionsFromText (item.Element(XName.Get("title")).Value) 
                              |> List.map (fun n -> {Id= -1; Name=n; IsFeatured=false})
                              |> List.rev
                              |> Set.ofList
                              |> Set.intersect (suggestionsFromText (item.Element(XName.Get("description")).Value) |> List.map (fun n -> {Id= -1; Name=n; IsFeatured=false}) |> Set.ofList)
                              |> Set.toList

                IsFeatured=  false
                Timestamp=   date
            }

        let url =    user.AccessId
        let uri =    Uri(url) 
        use client = httpClient <| sprintf "http://%s/" uri.Host

        let response = client.GetAsync(url) |> toResult
        let links = 
            if response.IsSuccessStatusCode
               then let text = response.Content.ReadAsStringAsync() |> toResult
                    XElement.Parse(text).Descendants(XName.Get("item")) 
                    |> Seq.toList
                    |> List.map toLink

               else []
        links