module Nikeza.Server.RSSFeed

    open System.Xml.Linq
    open System
    open Model
    open Http
    open Asynctify
    open StackOverflow

    let getThumbnail accessId =
        "to be implemented..."

    let suggestionsFromText (title:string) =
        let allTags = CachedTags.Instance() |> Set.ofList

        title.Split(' ')
        |> Array.map(fun w -> w.Trim().ToLower())
        |> Set.ofArray
        |> Set.intersect allTags 
        |> Set.toList

    let rssLinks (user:User) =
        let toLink (item:XElement) = { 
            Id =          -1
            ProfileId =   user.ProfileId
            Title=        item.Element(XName.Get("title")).Value
            Url=          item.Element(XName.Get("link")).Value
            Description = item.Element(XName.Get("description")).Value
            ContentType=  Podcast |> contentTypeToString
            Topics =      suggestionsFromText (item.Element(XName.Get("title")).Value) |> List.map (fun n -> {Id= -1; Name=n; IsFeatured=false})
                          |> Set.ofList
                          |> Set.intersect (suggestionsFromText (item.Element(XName.Get("description")).Value) |> List.map (fun n -> {Id= -1; Name=n; IsFeatured=false}) |> Set.ofList)
                          |> Set.toList
            IsFeatured=   false
         }

        let url =    user.AccessId
        let uri =    Uri(url) 
        use client = httpClient <| sprintf "http://%s/" uri.Host

        let response = client.GetAsync(url) |> toResult
        let links = 
            if response.IsSuccessStatusCode
               then let text = response.Content.ReadAsStringAsync()     |> toResult
                    XElement.Parse(text).Descendants(XName.Get("item")) |> Seq.map toLink
               else seq []
        links