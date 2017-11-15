module Nikeza.Server.ITunes

    open Model
    open Http
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

    let iTunesLinks (user:User) =
        let toLink (item:XElement) = { 
            Id =          -1
            ProfileId =   "to be derived..."
            Title=        item.Element(XName.Get("title")) |> string
            Url=          item.Element(XName.Get("link"))  |> string
            Description = item.Element(XName.Get("description")) |> string
            ContentType=  Podcast |> contentTypeToString
            Topics =      []
            IsFeatured=   false
         }

        let url =    user.AccessId
        let uri =    Uri(url) 
        use client = httpClient uri.Host

        let response = client.GetAsync(url) |> Async.AwaitTask 
                                            |> Async.RunSynchronously
        let links = 
            if response.IsSuccessStatusCode
               then let text = response.Content.ReadAsStringAsync()     |> Async.AwaitTask |> Async.RunSynchronously
                    XElement.Parse(text).Descendants(XName.Get("item")) |> Seq.map toLink
               else seq []
        links