module Nikeza.Server.ITunes

    open Model

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

    let iTunesLinks accessId =
         seq []