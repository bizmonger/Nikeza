module Nikeza.Server.RSSFeed

    open Model
    open Http
    open Asynctify
    open System.Xml.Linq
    open System

    let getThumbnail accessId =
        "to be implemented..."

    let toLink item profileId = { 
        Id=            -1
        ProfileId=     profileId 
        Title=         item.Title
        Description=   item.Description
        Url=           item.Url 
        Topics=        []
        ContentType=   Podcast |> contentTypeToString
        IsFeatured=    false
    }

    let rssLinks (user:User) =
        let toLink (item:XElement) = { 
            Id =          -1
            ProfileId =   user.ProfileId
            Title=        item.Element(XName.Get("title")).Value
            Url=          item.Element(XName.Get("link")).Value
            Description = item.Element(XName.Get("description")).Value
            ContentType=  Podcast |> contentTypeToString
            Topics =      []
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