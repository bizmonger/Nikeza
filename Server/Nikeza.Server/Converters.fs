﻿module Nikeza.Server.Converters

open Nikeza.Server.Model

let toPortfolio links : Portfolio = { 
    Answers=  links |> List.filter (fun l -> l.ContentType = (contentTypeToString Answer))
    Articles= links |> List.filter (fun l -> l.ContentType = (contentTypeToString Article))
    Videos=   links |> List.filter (fun l -> l.ContentType = (contentTypeToString Video))
    Podcasts= links |> List.filter (fun l -> l.ContentType = (contentTypeToString Podcast))
}

let toProfileRequest (profile:Profile) : ProfileRequest = {
    Id=        profile.Id
    FirstName= profile.FirstName
    LastName=  profile.LastName
    Email=     profile.Email
    ImageUrl=  profile.ImageUrl
    Bio=       profile.Bio
    Sources=   profile.Sources
}